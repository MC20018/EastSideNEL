using System.Text.Json;
using Codexus.Cipher.Entities.WPFLauncher;
using EastSide.Core.Entity.WFPLauncher;
using EastSide.Manager;
using EastSide.Handlers.PC.Account;
using EastSide.Entities.Web;
using EastSide.Type;

namespace EastSide.Handlers.PC.Login
{
    public class LoginCookie
    {
        public object Execute(string cookie)
        {
            try
            {
                EntityX19CookieRequest req;
                try
                {
                    req = JsonSerializer.Deserialize<EntityX19CookieRequest>(cookie) ?? new EntityX19CookieRequest { Json = cookie};
                }
                catch
                {
                    req = new EntityX19CookieRequest { Json = cookie };
                }
                var (authOtp, channel) = AppState.X19.LoginWithCookie(req);
                UserManager.Instance.AddUserToMaintain(authOtp);
                UserManager.Instance.AddUser(new EntityUser
                {
                    UserId = authOtp.EntityId,
                    Authorized = true,
                    AutoLogin = false,
                    Channel = channel,
                    Type = "cookie",
                    Details = cookie
                }, channel == "netease");
                var list = new System.Collections.ArrayList();
                list.Add(new { type = "Success_login", entityId = authOtp.EntityId, channel });
                var items = GetAccount.GetAccountItems();
                list.Add(new { type = "accounts", items });
                return list;
            }
            catch (ArgumentNullException)
            {
                return new { type = "login_error", message = "当前cookie过期了" };
            }
            catch (Exception ex)
            {
                return new { type = "login_error", message = ex.Message};
            }
        }
    }
}
