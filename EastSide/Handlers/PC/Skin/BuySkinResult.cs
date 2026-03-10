using System;
using EastSide.Manager;
using EastSide.Type;
using Serilog;

namespace EastSide.Handlers.PC.Skin;

public class BuySkinResultData
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool NotLogin { get; set; }
}

public class BuySkinResult
{
    public BuySkinResultData Execute(string accountId, string orderId, int buyType)
    {
        var user = UserManager.Instance.GetAvailableUser(accountId);
        if (user == null) return new BuySkinResultData { NotLogin = true };

        try
        {
            AppState.X19.BuyItemResult(user.UserId, user.AccessToken, orderId, buyType);
            return new BuySkinResultData { Success = true };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "确认购买皮肤失败");
            return new BuySkinResultData { Success = false, Message = "确认购买失败: " + ex.Message };
        }
    }
}
