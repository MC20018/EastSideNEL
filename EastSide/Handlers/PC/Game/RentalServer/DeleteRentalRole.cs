using System;
using System.Linq;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Entities.Web.RentalGame;
using Serilog;

namespace EastSide.Handlers.PC.Game.RentalServer;

public class DeleteRentalRole
{
    public RentalServerRolesResult Execute(string serverId, string entityId)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new RentalServerRolesResult { NotLogin = true };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(entityId))
            return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            AppState.X19.DeleteRentalGameRole(last.UserId, last.AccessToken, entityId);
            var entities = AppState.X19.GetRentalGameRolesList(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.EntityId, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除租赁服角色失败: serverId={ServerId}, entityId={EntityId}", serverId, entityId);
            return new RentalServerRolesResult { Success = false, Message = ex.Message };
        }
    }

    public RentalServerRolesResult ExecuteForAccount(string accountId, string serverId, string entityId)
    {
        if (string.IsNullOrWhiteSpace(accountId)) return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(entityId))
            return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            var u = UserManager.Instance.GetAvailableUser(accountId);
            if (u == null) return new RentalServerRolesResult { NotLogin = true };
            AppState.X19.DeleteRentalGameRole(u.UserId, u.AccessToken, entityId);
            var entities = AppState.X19.GetRentalGameRolesList(u.UserId, u.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.EntityId, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除租赁服角色失败: serverId={ServerId}, entityId={EntityId}", serverId, entityId);
            return new RentalServerRolesResult { Success = false, Message = ex.Message };
        }
    }
}
