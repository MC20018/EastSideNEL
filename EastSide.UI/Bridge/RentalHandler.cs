using System;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Entities.Web.RentalGame;
using EastSide.Handlers.PC.Game.RentalServer;
using EastSide.Handlers.PC.Account;
using EastSide.Handlers.PC.Game.NetGame;
using EastSide.Manager;
using Serilog;
using GetRoleNamed = EastSide.Handlers.PC.Game.RentalServer.GetRoleNamed;

namespace EastSide.UI.Bridge;

public static class RentalHandler
{
    public static async Task<BridgeResponse> ListServers(BridgeRequest req)
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

            var result = await Task.Run(() => new ListRentalServers().Execute(offset, pageSize));

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "获取失败");

            var items = result.Items.Select(s => new
            {
                entityId = s.EntityId,
                name = s.Name,
                playerCount = s.PlayerCount,
                hasPassword = s.HasPassword,
                mcVersion = s.McVersion,
                imageUrl = s.ImageUrl
            }).ToList();

            return BridgeResponse.Ok(req, new { items, hasMore = result.HasMore });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取租赁服列表失败");
            return BridgeResponse.Fail(req, "获取租赁服列表失败");
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
            Log.Error(ex, "获取租赁服角色失败");
            return BridgeResponse.Fail(req, "获取角色失败");
        }
    }

    public static async Task<BridgeResponse> CreateRole(BridgeRequest req)
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
            var result = await Task.Run(() =>
                string.IsNullOrWhiteSpace(accountId)
                    ? new CreateRentalRole().Execute(serverId, roleName)
                    : new CreateRentalRole().ExecuteForAccount(accountId!, serverId, roleName));

            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "创建角色失败");

            var roles = result.Items.Select(r => new { id = r.Id, name = r.Name }).ToList();
            return BridgeResponse.Ok(req, new { roles, message = "角色创建成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "创建租赁服角色失败");
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
            Log.Error(ex, "获取租赁服详情失败");
            return BridgeResponse.Fail(req, "获取服务器详情失败");
        }
    }

    public static async Task<BridgeResponse> DeleteRole(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        var roleId = req.Data.Value.GetProperty("roleId").GetString() ?? "";
        string? accountId = null;
        if (req.Data.Value.TryGetProperty("accountId", out var aEl))
            accountId = aEl.GetString();

        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleId))
            return BridgeResponse.Fail(req, "缺少参数");

        try
        {
            var result = await Task.Run(() =>
                string.IsNullOrWhiteSpace(accountId)
                    ? new DeleteRentalRole().Execute(serverId, roleId)
                    : new DeleteRentalRole().ExecuteForAccount(accountId!, serverId, roleId));

            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "删除角色失败");

            var roles = result.Items.Select(r => new { id = r.Id, name = r.Name }).ToList();
            return BridgeResponse.Ok(req, new { roles, message = "角色已删除" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除租赁服角色失败");
            return BridgeResponse.Fail(req, "删除角色失败");
        }
    }

    public static async Task<BridgeResponse> JoinServer(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var accountId = req.Data.Value.GetProperty("accountId").GetString() ?? "";
        var serverId = req.Data.Value.GetProperty("serverId").GetString() ?? "";
        var serverName = req.Data.Value.GetProperty("serverName").GetString() ?? "";
        var roleId = req.Data.Value.GetProperty("roleId").GetString() ?? "";
        var mcVersion = "";
        var password = "";
        if (req.Data.Value.TryGetProperty("mcVersion", out var vEl)) mcVersion = vEl.GetString() ?? "";
        if (req.Data.Value.TryGetProperty("password", out var pwEl)) password = pwEl.GetString() ?? "";

        if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleId))
            return BridgeResponse.Fail(req, "缺少必要参数");

        try
        {
            await Task.Run(() => new SelectAccount().Execute(accountId));

            var joinRequest = new EntityJoinRentalGame
            {
                ServerId = serverId,
                ServerName = serverName,
                Role = roleId,
                Password = password,
                McVersion = mcVersion,
                GameId = serverId
            };

            var result = await new JoinRentalGame().Execute(joinRequest);

            if (result.NotLogin) return BridgeResponse.Fail(req, "未登录游戏账号");
            if (!result.Success) return BridgeResponse.Fail(req, result.Message ?? "启动失败");

            RecentPlayManager.Instance.Add(new RecentEntry
            {
                ServerId = serverId,
                ServerName = serverName,
                Type = "rental",
                PlayTime = DateTime.Now,
                McVersion = mcVersion,
                HasPassword = !string.IsNullOrEmpty(password)
            });

            return BridgeResponse.Ok(req, new { ip = result.Ip, port = result.Port, message = "启动成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "加入租赁服失败");
            return BridgeResponse.Fail(req, "启动失败: " + ex.Message);
        }
    }
}
