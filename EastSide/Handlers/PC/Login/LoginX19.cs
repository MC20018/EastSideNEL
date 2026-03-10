using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codexus.Cipher.Protocol;
using Codexus.Cipher.Utils.Exception;
using EastSide.Entities.Web;
using EastSide.Entities.Web.NEL;
using EastSide.Handlers.PC.Account;
using EastSide.Manager;
using EastSide.Type;
using Serilog;

namespace EastSide.Handlers.PC.Login;

public class LoginX19
{
    public object Execute(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new { type = "login_x19_error", message = "邮箱或密码不能为空" };
            }

            InternalQuery.Initialize();
            
            WPFLauncher x = AppState.X19;
            var mPayUser = x.LoginWithEmailAsync(email, password).GetAwaiter().GetResult();
            var device = x.MPay.GetDevice();
            var cookieRequest = WPFLauncher.GenerateCookie(mPayUser, device);
            string peCookieJson = JsonSerializer.Serialize(cookieRequest);
            
            Log.Information("[PC LoginX19] Got PE cookie, now login to PC with it");
            
            var (authOtp, channel) = x.LoginWithCookie(peCookieJson);

            UserManager.Instance.AddUserToMaintain(authOtp);
            UserManager.Instance.AddUser(new EntityUser
            {
                UserId = authOtp.EntityId,
                Authorized = true,
                AutoLogin = false,
                Channel = channel,
                Type = "netease",
                Details = JsonSerializer.Serialize(new { email, password })
            });

            var list = new ArrayList();
            list.Add(new { type = "Success_login", entityId = authOtp.EntityId, channel });
            var items = GetAccount.GetAccountItems();
            list.Add(new { type = "accounts", items });
            return list;
        }
        catch (VerifyException ve)
        {
            Log.Error(ve, "[LoginX19] 验证失败");
            
            if (TryParseSecurityVerify(ve.Message) is { } verify)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = verify.VerifyUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception browserEx)
                {
                    Log.Warning(browserEx, "[LoginX19] 打开浏览器失败");
                }
                return new { type = "login_x19_verify", message = verify.Reason, verify_url = verify.VerifyUrl };
            }
            
            return new { type = "login_x19_error", message = ve.Message};
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[LoginX19] 登录异常");
            
            var msg = ex.Message;
            
            if (msg.Contains("response:") && msg.Contains("{"))
            {
                try
                {
                    var jsonStart = msg.IndexOf('{');
                    var jsonPart = msg.Substring(jsonStart);
                    
                    var errorResponse = JsonSerializer.Deserialize<LoginErrorResponse>(jsonPart);
                    if (errorResponse?.Reason != null)
                    {
                        var reason = errorResponse.Reason;
                        
                        if (reason.Contains("用户名格式错误") || reason.Contains("格式错误"))
                        {
                            return new { type = "login_x19_error", message = "邮箱格式错误" };
                        }
                        
                        return new { type = "login_x19_error", message = reason };
                    }
                }
                catch
                {
                }
            }
            
            if (msg.Contains("password") || msg.Contains("密码"))
            {
                return new { type = "login_x19_error", message = "邮箱或密码错误" };
            }
            
            return new { type = "login_x19_error", message = msg.Length == 0 ? "登录失败" : msg };
        }
    }
    
    private class LoginErrorResponse
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
        
        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
    
    private static EntitySecurityVerify? TryParseSecurityVerify(string message)
    {
        try
        {
            var entity = JsonSerializer.Deserialize<EntitySecurityVerify>(message);
            if (entity is { IsSecurityVerify: true, VerifyUrl.Length: > 0 })
            {
                entity.VerifyUrl = WebUtility.HtmlDecode(entity.VerifyUrl);
                return entity;
            }
        }
        catch
        {}
        return null;
    }
}
