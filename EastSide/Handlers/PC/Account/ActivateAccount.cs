using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codexus.Cipher.Utils.Exception;
using EastSide.Manager;
using EastSide.Handlers.PC.Login;
using Codexus.Cipher.Protocol;
using Serilog;

namespace EastSide.Handlers.PC.Account;

public class ActivateAccount
{
    public object Execute(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            return new { type = "activate_account_error", message = "缺少id" };
            
        var u = UserManager.Instance.GetUserByEntityId(id);
        if (u == null)
        {
            return new { type = "activate_account_error", message = "账号不存在" };
        }
        
        try
        {
            if (!u.Authorized)
            {
                var result = ReloginByType(u);
                var tProp = result?.GetType().GetProperty("type");
                var tVal = tProp?.GetValue(result) as string;
                
                if (tVal == "captcha_required" || tVal == "captcha_required_pe")
                {
                    Log.Information("[ActivateAccount] 需要验证码");
                    return result!;
                }
                
                if (tVal != null && tVal.EndsWith("_error", StringComparison.OrdinalIgnoreCase))
                {
                    var mProp = result?.GetType().GetProperty("message");
                    var msg = mProp?.GetValue(result) as string ?? "登录失败";
                    Log.Error("[ActivateAccount] 登录失败: {Msg}", msg);
                    return result!;
                }
                
                u.Authorized = true;
                UserManager.Instance.MarkDirtyAndScheduleSave();
            }
            
            var list = new System.Collections.ArrayList();
            var items = GetAccount.GetAccountItems();
            list.Add(new { type = "Success_login", entityId = u.UserId, channel = u.Channel });
            list.Add(new { type = "accounts", items });
            return list;
        }
        catch (CaptchaException)
        {
            if (u.Type?.ToLowerInvariant() == "password")
                return HandleCaptchaRequired(u);
            return new { type = "activate_account_error", message = "登录失败" };
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            var lower = msg.ToLowerInvariant();
            
            if (lower.Contains("parameter") && lower.Contains("'s'") && u.Type?.ToLowerInvariant() == "password")
            {
                return HandleCaptchaRequired(u);
            }
            
            return new { type = "activate_account_error", message = msg.Length == 0 ? "激活失败" : msg };
        }
    }

    private object ReloginByType(Entities.Web.EntityUser u)
    {
        var userType = u.Type?.ToLowerInvariant() ?? string.Empty;
        
        switch (userType)
        {
            case "password":
            case "4399":
                var jsonOptions = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };
                var pwdReq = JsonSerializer.Deserialize<Entities.Web.NEL.EntityPasswordRequest>(u.Details, jsonOptions);
                if (pwdReq == null) 
                    throw new Exception("无法解析4399登录信息");
                return new Login4399().Execute(pwdReq.Account, pwdReq.Password, null, null);

            case "netease":
            case "x19":
                var neteaseReq = JsonSerializer.Deserialize<NeteaseLoginInfo>(u.Details);
                if (neteaseReq == null) 
                    throw new Exception("无法解析网易登录信息");
                return new LoginX19().Execute(neteaseReq.Email, neteaseReq.Password);

            case "cookie":
                InternalQuery.Initialize();
                var (authOtp, channel) = Type.AppState.X19.LoginWithCookie(u.Details);
                UserManager.Instance.AddUserToMaintain(authOtp);
                return new { type = "Success_login", entityId = authOtp.EntityId, channel };

            default:
                throw new Exception($"不支持的账号类型: {u.Type}");
        }
    }

    private class NeteaseLoginInfo
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    private object HandleCaptchaRequired(Entities.Web.EntityUser u)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            var req = JsonSerializer.Deserialize<Entities.Web.NEL.EntityPasswordRequest>(u.Details, jsonOptions);
            var captchaSid = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N").Substring(0, 8);
            var url = "https://ptlogin.4399.com/ptlogin/captcha.do?captchaId=" + captchaSid;

            return new 
            { 
                type = "captcha_required", 
                account = req?.Account ?? string.Empty, 
                password = req?.Password ?? string.Empty, 
                sessionId = captchaSid, 
                captchaUrl = url 
            };
        }
        catch
        {
            return new { type = "captcha_required" };
        }
    }
}
