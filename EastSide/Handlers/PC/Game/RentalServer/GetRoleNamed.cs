using System;
using System.Linq;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Entities.Web.RentalGame;
using Serilog;

namespace EastSide.Handlers.PC.Game.RentalServer;

public class GetRoleNamed
{
    public RentalServerRolesResult Execute(string serverId)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null)
        {
            return new RentalServerRolesResult { NotLogin = true };
        }
        if (string.IsNullOrWhiteSpace(serverId))
        {
            return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        }
        try
        {
            var entities = AppState.X19.GetRentalGameRolesList(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.EntityId, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取租赁服角色失败: serverId={ServerId}", serverId);
            return new RentalServerRolesResult { Success = false, Message = "获取失败" };
        }
    }

    public RentalServerRolesResult ExecuteForAccount(string accountId, string serverId)
    {
        if (string.IsNullOrWhiteSpace(accountId)) return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        if (string.IsNullOrWhiteSpace(serverId)) return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            var u = UserManager.Instance.GetAvailableUser(accountId);
            if (u == null) return new RentalServerRolesResult { NotLogin = true };
            var entities = AppState.X19.GetRentalGameRolesList(u.UserId, u.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.EntityId, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取租赁服角色失败: serverId={ServerId}", serverId);
            return new RentalServerRolesResult { Success = false, Message = "获取失败" };
        }
    }
}
