using EastSide.Manager;
using EastSide.Type;
using Serilog;
using System.Text.Json;

namespace EastSide.Handlers.PC.Skin;

public class SetSkinResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool NotLogin { get; set; }
}

public class SetSkin
{
    public SetSkinResult Execute(string entityId)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new SetSkinResult { NotLogin = true };
        return ApplySkin(last.UserId, last.AccessToken, entityId);
    }

    public SetSkinResult ExecuteForAccount(string accountId, string skinEntityId)
    {
        var user = UserManager.Instance.GetAvailableUser(accountId);
        if (user == null) return new SetSkinResult { NotLogin = true };
        return ApplySkin(user.UserId, user.AccessToken, skinEntityId);
    }

    private static SetSkinResult ApplySkin(string userId, string accessToken, string entityId)
    {
        try
        {
            dynamic r = AppState.X19.SetSkin(userId, accessToken, entityId);
            int code = 0;
            string msg = string.Empty;
            try
            {
                code = Convert.ToInt32(r.Code);
                msg = r.Message ?? string.Empty;
            }
            catch {  }
            var succ = code == 0;
            return new SetSkinResult { Success = succ, Message = msg };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "设置皮肤失败");
            return new SetSkinResult { Success = false, Message = "设置失败" };
        }
    }
}
