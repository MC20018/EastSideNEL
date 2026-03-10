using System;
using System.Linq;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Entities.Web.RentalGame;
using Serilog;

namespace EastSide.Handlers.PC.Game.RentalServer;

public class CreateRentalRole
{
    public RentalServerRolesResult Execute(string serverId, string roleName)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null)
        {
            return new RentalServerRolesResult { NotLogin = true };
        }
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
        {
            return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        }
        try
        {
            var result = AppState.X19.AddRentalGameRole(last.UserId, last.AccessToken, serverId, roleName);
            
            if (result.Code != 0)
            {
                Log.Error("[RentalServer] 创建角色失败: {Message}", result.Message);
                return new RentalServerRolesResult { Success = false, Message = result.Message ?? "创建失败" };
            }

            var entities = AppState.X19.GetRentalGameRolesList(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[RentalServer] 创建租赁服角色失败: serverId={ServerId}", serverId);
            return new RentalServerRolesResult { Success = false, Message = "创建失败" };
        }
    }

    public RentalServerRolesResult ExecuteForAccount(string accountId, string serverId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(accountId)) return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
        {
            return new RentalServerRolesResult { Success = false, Message = "参数错误" };
        }
        try
        {
            var u = UserManager.Instance.GetAvailableUser(accountId);
            if (u == null) return new RentalServerRolesResult { NotLogin = true };

            var result = AppState.X19.AddRentalGameRole(u.UserId, u.AccessToken, serverId, roleName);
            
            if (result.Code != 0)
            {
                Log.Error("[RentalServer] 创建角色失败: {Message}", result.Message);
                return new RentalServerRolesResult { Success = false, Message = result.Message ?? "创建失败" };
            }

            var entities = AppState.X19.GetRentalGameRolesList(u.UserId, u.AccessToken, serverId);
            var items = entities.Data.Select(r => new RentalRoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new RentalServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[RentalServer] 创建租赁服角色失败: serverId={ServerId}", serverId);
            return new RentalServerRolesResult { Success = false, Message = "创建失败" };
        }
    }
}
