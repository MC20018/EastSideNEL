using System;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Handlers.PC.Skin;
using EastSide.Manager;
using Serilog;

namespace EastSide.UI.Bridge;

public static class SkinHandler
{
    public static async Task<BridgeResponse> ListSkins(BridgeRequest req)
    {
        await Backend.WaitForInitAsync();
        try
        {
            var offset = 0;
            var pageSize = 20;

            if (req.Data != null)
            {
                if (req.Data.Value.TryGetProperty("offset", out var oEl)) offset = oEl.GetInt32();
                if (req.Data.Value.TryGetProperty("pageSize", out var pEl)) pageSize = pEl.GetInt32();
            }

            var result = await Task.Run(() => new GetFreeSkin().Execute(offset, pageSize));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取失败");

            var items = result.Items.Select(s => new
            {
                entityId = s.EntityId,
                name = s.Name,
                previewUrl = s.PreviewUrl
            }).ToList();

            return BridgeResponse.Ok(req, new { items, hasMore = result.HasMore });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取皮肤列表失败");
            return BridgeResponse.Fail(req, "获取皮肤列表失败");
        }
    }

    public static async Task<BridgeResponse> SearchSkins(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var keyword = "";
        if (req.Data.Value.TryGetProperty("keyword", out var kEl)) keyword = kEl.GetString() ?? "";
        if (string.IsNullOrWhiteSpace(keyword)) return BridgeResponse.Fail(req, "请输入搜索关键词");

        try
        {
            var result = await Task.Run(() => new SearchSkin().Execute(keyword));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "搜索失败");

            var items = result.Items.Select(s => new
            {
                entityId = s.EntityId,
                name = s.Name,
                previewUrl = s.PreviewUrl
            }).ToList();

            return BridgeResponse.Ok(req, new { items, hasMore = false });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "搜索皮肤失败");
            return BridgeResponse.Fail(req, "搜索皮肤失败");
        }
    }

    public static async Task<BridgeResponse> ApplySkin(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var skinId = req.Data.Value.GetProperty("skinId").GetString() ?? "";
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";

        if (string.IsNullOrWhiteSpace(skinId) || string.IsNullOrWhiteSpace(accountId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            var result = await Task.Run(() => new SetSkin().ExecuteForAccount(accountId, skinId));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "应用失败");

            return BridgeResponse.Ok(req, new { message = "皮肤应用成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "应用皮肤失败");
            return BridgeResponse.Fail(req, "应用皮肤失败");
        }
    }

    public static async Task<BridgeResponse> Purchase(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var skinId = req.Data.Value.GetProperty("skinId").GetString() ?? "";
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";

        if (string.IsNullOrWhiteSpace(skinId) || string.IsNullOrWhiteSpace(accountId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            var result = await Task.Run(() => new PurchaseSkin().Execute(accountId, skinId));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "购买失败");

            return BridgeResponse.Ok(req, new { message = "购买成功", data = result.Data });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "购买皮肤失败");
            return BridgeResponse.Fail(req, "购买皮肤失败");
        }
    }

    public static async Task<BridgeResponse> BuyResult(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";
        var orderId = req.Data.Value.GetProperty("orderId").GetString() ?? "";
        var buyType = 0;
        if (req.Data.Value.TryGetProperty("buyType", out var btEl)) buyType = btEl.GetInt32();

        if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(orderId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            var result = await Task.Run(() => new BuySkinResult().Execute(accountId, orderId, buyType));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "确认购买失败");

            return BridgeResponse.Ok(req, new { message = "确认成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "确认购买皮肤失败");
            return BridgeResponse.Fail(req, "确认购买失败");
        }
    }

    public static async Task<BridgeResponse> GetDetail(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var skinId = req.Data.Value.GetProperty("skinId").GetString() ?? "";
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";

        if (string.IsNullOrWhiteSpace(skinId) || string.IsNullOrWhiteSpace(accountId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            var result = await Task.Run(() => new GetSkinDetail().Execute(accountId, skinId));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取详情失败");

            return BridgeResponse.Ok(req, new
            {
                entityId = result.Item!.EntityId,
                name = result.Item.Name,
                previewUrl = result.Item.PreviewUrl
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取皮肤详情失败");
            return BridgeResponse.Fail(req, "获取皮肤详情失败");
        }
    }
}
