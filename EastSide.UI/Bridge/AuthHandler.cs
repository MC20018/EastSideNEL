using System;
using System.Text.Json;
using System.Threading.Tasks;
using EastSide.Manager;

namespace EastSide.UI.Bridge;

public static class AuthHandler
{
    public static async Task<BridgeResponse> GetStatus(BridgeRequest req)
    {
        var auth = AuthManager.Instance;

        if (auth.IsLoggedIn && (string.IsNullOrWhiteSpace(auth.Username)
            || auth.UserId <= 0
            || string.IsNullOrWhiteSpace(auth.Avatar)))
        {
            await auth.FetchUserInfoAsync();
            auth.SaveToDisk();
        }

        var status = auth.GetMembershipStatus();

        return BridgeResponse.Ok(req, new
        {
            isLoggedIn = auth.IsLoggedIn,
            username = auth.Username,
            email = auth.Email,
            userId = auth.UserId,
            avatar = auth.Avatar,
            rank = auth.Rank,
            isAdmin = auth.IsAdmin,
            membershipStatus = status.ToString(),
            expiryDate = auth.GetFormattedExpiryDate(),
            daysLeft = auth.GetFormattedDaysLeft(),
            hasFeatureAccess = auth.HasFeatureAccess()
        });
    }

    public static async Task<BridgeResponse> Login(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var username = req.Data.Value.GetProperty("username").GetString() ?? "";
        var password = req.Data.Value.GetProperty("password").GetString() ?? "";

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return BridgeResponse.Fail(req, "用户名和密码不能为空");

        var result = await AuthManager.Instance.LoginAsync(username, password);
        if (!result.Success)
            return BridgeResponse.Fail(req, result.Message);

        _ = Task.Run(async () =>
        {
            try
            {
                await AuthManager.Instance.FetchUserInfoAsync();
                AuthManager.Instance.SaveToDisk();
                PushAuthUpdate();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "后台拉取用户信息失败");
            }
        });

        return BridgeResponse.Ok(req, new
        {
            isLoggedIn = true,
            username = username,
            email = (string?)null,
            userId = 0,
            avatar = (string?)null,
            rank = (string?)null,
            isAdmin = false,
            membershipStatus = "Unknown",
            expiryDate = "",
            daysLeft = "",
            hasFeatureAccess = false,
            fetchingUserInfo = true
        });
    }

    public static BridgeResponse Logout(BridgeRequest req)
    {
        AuthManager.Instance.Clear();
        return BridgeResponse.Ok(req, new { message = "已退出登录" });
    }

    public static async Task<BridgeResponse> SendRegisterCode(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var email = req.Data.Value.GetProperty("email").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(email))
            return BridgeResponse.Fail(req, "请输入邮箱");

        var result = await AuthManager.Instance.SendRegisterMailAsync(email);
        return result.Success
            ? BridgeResponse.Ok(req, new { message = result.Message })
            : BridgeResponse.Fail(req, result.Message);
    }

    public static async Task<BridgeResponse> VerifyCode(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var email = req.Data.Value.GetProperty("email").GetString() ?? "";
        var code = req.Data.Value.GetProperty("code").GetString() ?? "";

        var result = await AuthManager.Instance.VerifyCodeAsync(email, code);
        return result.Success
            ? BridgeResponse.Ok(req, new { message = result.Message })
            : BridgeResponse.Fail(req, result.Message);
    }

    public static async Task<BridgeResponse> CompleteRegister(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var email = req.Data.Value.GetProperty("email").GetString() ?? "";
        var username = req.Data.Value.GetProperty("username").GetString() ?? "";
        var password = req.Data.Value.GetProperty("password").GetString() ?? "";

        var result = await AuthManager.Instance.RegisterNextAsync(email, username, password);
        if (!result.Success)
            return BridgeResponse.Fail(req, result.Message);

        return await GetStatus(req);
    }

    private static void PushAuthUpdate()
    {
        try
        {
            var auth = AuthManager.Instance;
            var status = auth.GetMembershipStatus();
            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                action = "auth:updated",
                requestId = "",
                success = true,
                data = new
                {
                    isLoggedIn = auth.IsLoggedIn,
                    username = auth.Username,
                    email = auth.Email,
                    userId = auth.UserId,
                    avatar = auth.Avatar,
                    rank = auth.Rank,
                    isAdmin = auth.IsAdmin,
                    membershipStatus = status.ToString(),
                    expiryDate = auth.GetFormattedExpiryDate(),
                    daysLeft = auth.GetFormattedDaysLeft(),
                    hasFeatureAccess = auth.HasFeatureAccess(),
                    fetchingUserInfo = false
                }
            });
            AppWindow.Instance?.SendWebMessage(json);
        }
        catch (Exception ex)
        {
            Serilog.Log.Debug(ex, "推送认证状态更新失败");
        }
    }

    public static async Task<BridgeResponse> ActivateCard(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var cardKey = req.Data.Value.GetProperty("cardKey").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(cardKey))
            return BridgeResponse.Fail(req, "请输入卡密");

        var token = AuthManager.Instance.Token;
        if (string.IsNullOrWhiteSpace(token))
            return BridgeResponse.Fail(req, "请先登录");

        var response = await EastSide.Core.Api.OxygenApi.Instance.ActivateCardKeyAsync(token, cardKey);
        if (!response.Success)
            return BridgeResponse.Fail(req, response.Message ?? "激活失败");

        await AuthManager.Instance.RefreshFeatureAccessAsync();
        return await GetStatus(req);
    }
}
