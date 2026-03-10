using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Entities.Web.NetGame;
using EastSide.Handlers.PC.Game.NetGame;
using EastSide.Handlers.PC.Account;
using EastSide.Manager;
using Serilog;

namespace EastSide.UI.Bridge;

public static class NetworkHandler
{
    public static async Task<BridgeResponse> ListServers(BridgeRequest req)
    {
        await Backend.WaitForInitAsync();
        try
        {
            var offset = 0;
            var pageSize = 20;
            string? keyword = null;

            if (req.Data != null)
            {
                if (req.Data.Value.TryGetProperty("offset", out var oEl)) offset = oEl.GetInt32();
                if (req.Data.Value.TryGetProperty("pageSize", out var pEl)) pageSize = pEl.GetInt32();
                if (req.Data.Value.TryGetProperty("keyword", out var kEl)) keyword = kEl.GetString();
            }

            var result = await Task.Run(() =>
                string.IsNullOrWhiteSpace(keyword)
                    ? new ListServers().Execute(offset, pageSize)
                    : new SearchServers().Execute(keyword!, offset, pageSize));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取失败");

            var items = result.Items.Select(s => new
            {
                entityId = s.EntityId,
                name = s.Name,
                onlineCount = s.OnlineCount,
                imageUrl = s.ImageUrl
            }).ToList();

            return BridgeResponse.Ok(req, new { items, hasMore = result.HasMore });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取服务器列表失败");
            return BridgeResponse.Fail(req, "获取服务器列表失败");
        }
    }
    public static async Task<BridgeResponse> GetRoles(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(serverId)) return BridgeResponse.Fail(req, "缺少 serverId");

        try
        {
            string? accountId = null;
            if (req.Data.Value.TryGetProperty("accountId", out var aEl))
                accountId = aEl.GetString();

            var result = await Task.Run(() =>
                string.IsNullOrWhiteSpace(accountId)
                    ? new GetRoleNamed().Execute(serverId)
                    : new GetRoleNamed().ExecuteForAccount(accountId!, serverId));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取角色失败");

            var roles = result.Items.Select(r =>
            {
                var userId = string.IsNullOrWhiteSpace(accountId)
                    ? UserManager.Instance.GetLastAvailableUserId()
                    : UserManager.Instance.GetAvailableUserId(accountId);
                var ban = userId != null
                    ? BanRecordManager.Instance.GetBanEntry(userId, serverId, r.Name)
                    : null;
                return new
                {
                    id = r.Id, name = r.Name,
                    banned = ban != null && (ban.IsPermanent || (ban.UnbanTime != null && DateTime.Now < ban.UnbanTime.Value)),
                    permanent = ban?.IsPermanent ?? false,
                    unbanTime = ban?.UnbanTime?.ToString("o")
                };
            }).ToList();
            return BridgeResponse.Ok(req, new { roles });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取角色列表失败");
            return BridgeResponse.Fail(req, "获取角色列表失败");
        }
    }

    public static async Task<BridgeResponse> CreateRole(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        var roleName = req.Data.Value.GetProperty("roleName").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
            return BridgeResponse.Fail(req, "缺少参数");

        try
        {
            var result = await Task.Run(() => new CreateRoleNamed().Execute(serverId, roleName));
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "创建角色失败");

            var roles = result.Items.Select(r => new { id = r.Id, name = r.Name }).ToList();
            return BridgeResponse.Ok(req, new { roles, message = "角色创建成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "创建角色失败");
            return BridgeResponse.Fail(req, "创建角色失败");
        }
    }

    public static async Task<BridgeResponse> GetDetail(BridgeRequest req)
    {
        await Backend.WaitForInitAsync();
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(serverId)) return BridgeResponse.Fail(req, "缺少 serverId");

        try
        {
            var result = await Task.Run(() => new GetServersDetail().Execute(serverId));
            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取失败");
            return BridgeResponse.Ok(req, new { images = result.Images, description = result.Description });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取服务器详情失败");
            return BridgeResponse.Fail(req, "获取服务器详情失败");
        }
    }

    public static async Task<BridgeResponse> DeleteRole(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        var roleName = req.Data.Value.GetProperty("roleName").GetString() ?? "";
        string? accountId = null;
        if (req.Data.Value.TryGetProperty("accountId", out var aEl))
            accountId = aEl.GetString();

        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
            return BridgeResponse.Fail(req, "缺少参数");

        try
        {
            var result = string.IsNullOrWhiteSpace(accountId)
                ? await new DeleteRoleNamed().Execute(serverId, roleName)
                : await new DeleteRoleNamed().ExecuteForAccount(accountId!, serverId, roleName);

            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "删除角色失败");

            var roles = result.Items.Select(r => new { id = r.Id, name = r.Name }).ToList();
            return BridgeResponse.Ok(req, new { roles, message = "角色已删除" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除角色失败");
            return BridgeResponse.Fail(req, "删除角色失败");
        }
    }

    public static async Task<BridgeResponse> GetAccounts(BridgeRequest req)
    {
        await Backend.WaitForInitAsync();
        try
        {
            var accounts = UserManager.Instance.GetAuthorizedAccounts();
            var list = accounts.Select(a => new { id = a.Id, label = a.Label }).ToList();
            return BridgeResponse.Ok(req, new { accounts = list });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取已授权账号失败");
            return BridgeResponse.Fail(req, "获取账号失败");
        }
    }

    public static async Task<BridgeResponse> SelectAccount(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(accountId)) return BridgeResponse.Fail(req, "缺少 accountId");

        try
        {
            await Task.Run(() => new Handlers.PC.Account.SelectAccount().Execute(accountId));
            return BridgeResponse.Ok(req, new { message = "已切换" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "切换账号失败");
            return BridgeResponse.Fail(req, "切换账号失败");
        }
    }

    public static async Task<BridgeResponse> JoinServer(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        var serverName = req.Data.Value.GetProperty("serverName").GetString() ?? "";
        var roleId = req.Data.Value.GetProperty("roleId").GetString() ?? "";

        if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            var result = await new JoinGame().Execute(accountId, serverId, serverName, roleId);

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "启动失败");

            RecentPlayManager.Instance.Add(new RecentEntry
            {
                ServerId = serverId,
                ServerName = serverName,
                Type = "network",
                PlayTime = DateTime.Now
            });

            return BridgeResponse.Ok(req, new { ip = result.Ip, port = result.Port, message = "启动成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "加入服务器失败");
            return BridgeResponse.Fail(req, "启动失败: " + ex.Message);
        }
    }
}
