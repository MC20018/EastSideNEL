using System;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Protocol;
using Codexus.Cipher.Utils.Exception;
using EastSide.Core.Utils;
using EastSide.Entities.Web.NEL;
using EastSide.Handlers.PC.Account;
using EastSide.Manager;
using EastSide.Type;
using EastSide.Utils;
using Serilog;

namespace EastSide.Handlers.PC.Login
{
    public class Login4399
    {
        public object Execute(string account, string password, string? captchaIdentifier = null, string? captcha = null)
        {
            try
            {
                InternalQuery.Initialize();
                
                string peCookie = AppState.Com4399.LoginAndAuthorize(account, password, captcha, captchaIdentifier).GetAwaiter().GetResult();
                string pcCookie = new Pc4399().LoginWithPasswordAsync(account, password, captcha, captchaIdentifier).GetAwaiter().GetResult();

                bool useMixed = SettingManager.Instance.Get().UseMixedLogin;
                string loginCookie = useMixed ? peCookie : pcCookie;

                if (string.IsNullOrWhiteSpace(loginCookie))
                {
                    return new { type = "login_4399_error", message = useMixed ? "Failed to get PE cookie" : "Failed to get PC cookie" };
                }

                Log.Debug("[PC Login4399] UseMixedLogin={UseMixed}, logging in with {Type} cookie", useMixed, useMixed ? "PE" : "PC");

                var (authOtp, channel) = AppState.X19.LoginWithCookie(loginCookie);
                Log.Information("Login with password: {UserId} Channel: {LoginChannel}", authOtp.EntityId, channel);
                Log.Debug("User details: {UserId},{Token}", authOtp.EntityId, authOtp.Token);
                
                UserManager.Instance.AddUserToMaintain(authOtp);
                UserManager.Instance.AddUser(new Entities.Web.EntityUser
                {
                    UserId = authOtp.EntityId,
                    Authorized = true,
                    AutoLogin = false,
                    Channel = channel,
                    Type = "password",
                    Details = JsonSerializer.Serialize(new EntityPasswordRequest
                    {
                        Account = account,
                        Password = password
                    })
                });
                
                var list = new System.Collections.ArrayList();
                list.Add(new { type = "Success_login", entityId = authOtp.EntityId, channel });
                var items = GetAccount.GetAccountItems();
                list.Add(new { type = "accounts", items });
                return list;
            }
            catch (CaptchaException ex)
            {
                Log.Warning(ex, "4399 登录需要验证码. account={Account}", account);
                return HandleCaptchaRequired(account, password);
            }
            catch (NullReferenceException ex)
            {
                Log.Warning("检测到 NullReferenceException，判定为需要实名认证: {Message}", ex.Message);
                return HandleRealnameRequired(account, password);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var lower = msg.ToLowerInvariant();
                Log.Error(ex, "4399 登录失败. account={Account}", account);
                Log.Information("4399 登录失败信息: {Message}", ex.Message);
                
                if (lower.Contains("实名") || lower.Contains("realname"))
                {
                    Log.Warning("检测到实名认证错误，启动实名认证流程");
                    return HandleRealnameRequired(account, password);
                }
                
                if (lower.Contains("unactived"))
                {
                    return "账号未激活";
                }
                if (lower.Contains("parameter") && lower.Contains("'s'"))
                {
                    return HandleCaptchaRequired(account, password);
                }
                if (lower.Contains("sessionid"))
                {
                    return new { type = "login_4399_error", message = "账号或密码错误" };
                }
                return new { type = "login_4399_error", message = string.IsNullOrEmpty(msg) ? "登录失败" : msg };
            }
        }
        
        private object HandleRealnameRequired(string account, string password)
        {
            try
            {
                Log.Information("开始4399实名认证流程...");
                
                var realnameTool = new RealNameTool();
                
                if (!realnameTool.LoadSfzFromEmbeddedResource())
                {
                    Log.Error("加载身份证信息失败");
                    return new { type = "login_4399_error", message = "实名认证失败：无法加载身份证数据" };
                }
                
                var task = realnameTool.RunFromFileAsync(account, password);
                task.Wait();
                
                Log.Information("实名认证流程完成，尝试重新登录...");
                
                System.Threading.Thread.Sleep(2000);
                
                return Execute(account, password);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "实名认证流程失败");
                return new { type = "login_4399_error", message = $"实名认证失败: {ex.Message}" };
            }
        }

        private object HandleCaptchaRequired(string account, string password)
        {
            var captchaSid = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N").Substring(0, 8);
            var url = "https://ptlogin.4399.com/ptlogin/captcha.do?captchaId=" + captchaSid;
            
            try
            {
                var recognizedCaptcha = CaptchaRecognitionService.RecognizeFromUrlAsync(url).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(recognizedCaptcha))
                {
                    Log.Information("[Login4399] 验证码自动识别成功: {Captcha}，正在重试登录", recognizedCaptcha);
                    return Execute(account, password, captchaSid, recognizedCaptcha);
                }
            }
            
            catch (Exception ex)
            {
                Log.Warning("[Login4399] 验证码自动识别失败: {Error}", ex.Message);
            }
            return new { type = "captcha_required", account, password, captchaIdentifier = captchaSid, captchaUrl = url };
        }
    }
}
