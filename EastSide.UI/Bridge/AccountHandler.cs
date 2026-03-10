using System;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Manager;
using EastSide.Handlers.PC.Account;
using Serilog;

namespace EastSide.UI.Bridge;

public static class AccountHandler
{
    public static async Task<BridgeResponse> GetAccounts(BridgeRequest req)
    {
        try
        {
            await EastSide.Backend.WaitForInitAsync();
            var pcList = GetAccount.GetAccountList();
            var accounts = pcList.Select(a => new
            {
                entityId = a.EntityId,
                channel = a.Channel,
                status = a.Status,
                alias = a.Alias,
                type = a.Type
            }).OrderBy(a => a.entityId).ToList();

            return BridgeResponse.Ok(req, new { accounts });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取账号列表失败");
            return BridgeResponse.Fail(req, "获取账号列表失败");
        }
    }

    public static async Task<BridgeResponse> Activate(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var entityId = req.Data.Value.GetProperty("entityId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(entityId)) return BridgeResponse.Fail(req, "缺少 entityId");

        try
        {
            var pcUser = UserManager.Instance.GetUserByEntityId(entityId);
            if (pcUser == null) return BridgeResponse.Fail(req, "未找到该账号");

            var result = await Task.Run(() => new ActivateAccount().Execute(entityId));
            if (result == null) return BridgeResponse.Fail(req, "激活失败");

            var resultType = result.GetType();
            var typeProp = resultType.GetProperty("type");
            if (typeProp != null)
            {
                var typeValue = typeProp.GetValue(result)?.ToString() ?? "";
                if (typeValue.EndsWith("_error", StringComparison.OrdinalIgnoreCase))
                {
                    var msg = resultType.GetProperty("message")?.GetValue(result)?.ToString() ?? "激活失败";
                    return BridgeResponse.Fail(req, msg);
                }
                if (typeValue.StartsWith("captcha_required"))
                    return BridgeResponse.Fail(req, "该账号需要验证码，请重新登录");
            }

            if (result is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var tv = item?.GetType().GetProperty("type")?.GetValue(item)?.ToString();
                    if (tv == "Success_login")
                        return BridgeResponse.Ok(req, new { message = "激活成功" });
                }
            }

            return BridgeResponse.Fail(req, "激活失败");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "激活账号失败: {EntityId}", entityId);
            return BridgeResponse.Fail(req, ex.Message);
        }
    }

    public static BridgeResponse Logout(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var entityId = req.Data.Value.GetProperty("entityId").GetString() ?? "";

        try
        {
            UserManager.Instance.RemoveAvailableUser(entityId);
            return BridgeResponse.Ok(req, new { message = "已注销" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "注销失败: {EntityId}", entityId);
            return BridgeResponse.Fail(req, ex.Message);
        }
    }

    public static async Task<BridgeResponse> Delete(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var entityId = req.Data.Value.GetProperty("entityId").GetString() ?? "";

        try
        {
            UserManager.Instance.RemoveAvailableUser(entityId);
            UserManager.Instance.RemoveUser(entityId);
            return BridgeResponse.Ok(req, new { message = "已删除" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除账号失败: {EntityId}", entityId);
            return BridgeResponse.Fail(req, ex.Message);
        }
    }

    public static async Task<BridgeResponse> UpdateAlias(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var entityId = req.Data.Value.GetProperty("entityId").GetString() ?? "";
        var alias = req.Data.Value.GetProperty("alias").GetString() ?? "";

        try
        {
            UserManager.Instance.UpdateUserAlias(entityId, alias);
            await UserManager.Instance.SaveUsersToDiskAsync();
            return BridgeResponse.Ok(req, new { message = "已保存" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "更新备注失败: {EntityId}", entityId);
            return BridgeResponse.Fail(req, ex.Message);
        }
    }

    public static async Task<BridgeResponse> RandomLogin(BridgeRequest req)
    {
        try
        {
            var (success, message) = await EastSide.Backend.RandomLogin4399Async();
            return success
                ? BridgeResponse.Ok(req, new { message })
                : BridgeResponse.Fail(req, message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "随机登录失败");
            return BridgeResponse.Fail(req, ex.Message);
        }
    }

    public static async Task<BridgeResponse> LoginAdd(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var method = req.Data.Value.GetProperty("method").GetString() ?? "";

        try
        {
            (bool success, string message) result;

            switch (method)
            {
                case "4399":
                {
                    var account = req.Data.Value.GetProperty("account").GetString() ?? "";
                    var password = req.Data.Value.GetProperty("password").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
                        return BridgeResponse.Fail(req, "请输入账号和密码");
                    result = await EastSide.Backend.Login4399Async(account, password);
                    break;
                }
                case "netease":
                {
                    var email = req.Data.Value.GetProperty("account").GetString() ?? "";
                    var password = req.Data.Value.GetProperty("password").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                        return BridgeResponse.Fail(req, "请输入邮箱和密码");
                    result = await EastSide.Backend.LoginNeteaseAsync(email, password);
                    break;
                }
                case "cookie":
                {
                    var cookie = req.Data.Value.GetProperty("cookie").GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(cookie))
                        return BridgeResponse.Fail(req, "请输入 Cookie");
                    result = await EastSide.Backend.LoginCookieAsync(cookie);
                    break;
                }
                default:
                    return BridgeResponse.Fail(req, $"未知登录方式: {method}");
            }

            return result.success
                ? BridgeResponse.Ok(req, new { message = result.message })
                : BridgeResponse.Fail(req, result.message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "添加账号失败");
            return BridgeResponse.Fail(req, ex.Message);
        }
    }
}
