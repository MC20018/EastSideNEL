using System;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Skin;
using EastSide.Manager;
using EastSide.Type;
using Serilog;

namespace EastSide.Handlers.PC.Skin;

public class PurchaseSkinResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool NotLogin { get; set; }
    public string? OrderId { get; set; }
    public object? Data { get; set; }
}

public class PurchaseSkin
{
    public PurchaseSkinResult Execute(string accountId, string itemId)
    {
        var user = UserManager.Instance.GetAvailableUser(accountId);
        if (user == null) return new PurchaseSkinResult { NotLogin = true };

        try
        {
            var entity = AppState.X19.PurchaseSkin(user.UserId, user.AccessToken, itemId);
            if (entity.Code != 0)
            {
                var msg = entity.Code == 35
                    ? "混合登录(Mixed)的账户暂时无法购买/设置皮肤"
                    : (entity.Message ?? "购买失败");
                return new PurchaseSkinResult { Success = false, Message = msg };
            }

            return new PurchaseSkinResult { Success = true, Data = entity.Data };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "购买皮肤失败");
            return new PurchaseSkinResult { Success = false, Message = "购买失败: " + ex.Message };
        }
    }
}
