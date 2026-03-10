using System;
using System.Linq;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Entity.WFPLauncher.NetGame;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Entities.Web.NetGame;
using Serilog;

namespace EastSide.Handlers.PC.Game.NetGame;

public class GetRoleNamed
{
    public ServerRolesResult Execute(string serverId)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new ServerRolesResult { NotLogin = true };
        if (string.IsNullOrWhiteSpace(serverId))
        {
            return new ServerRolesResult { Success = false, Message = "参数错误" };
        }
        try
        {
            if (SettingManager.Instance.Get().Debug) Log.Information("打开服务器: serverId={ServerId}, account={AccountId}", serverId, last.UserId);
            Entities<EntityGameCharacter> entities = AppState.X19.QueryNetGameCharacters(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new ServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取服务器角色失败: serverId={ServerId}", serverId);
            return new ServerRolesResult { Success = false, Message = "获取失败" };
        }
    }

    public ServerRolesResult ExecuteForAccount(string accountId, string serverId)
    {
        if (string.IsNullOrWhiteSpace(accountId)) return new ServerRolesResult { Success = false, Message = "参数错误" };
        if (string.IsNullOrWhiteSpace(serverId)) return new ServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            var u = UserManager.Instance.GetAvailableUser(accountId);
            if (u == null) return new ServerRolesResult { NotLogin = true };
            if (SettingManager.Instance.Get().Debug) Log.Information("打开服务器: serverId={ServerId}, account={AccountId}", serverId, u.UserId);
            Entities<EntityGameCharacter> entities = AppState.X19.QueryNetGameCharacters(u.UserId, u.AccessToken, serverId);
            var items = entities.Data.Select(r => new RoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new ServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取服务器角色失败: serverId={ServerId}", serverId);
            return new ServerRolesResult { Success = false, Message = "获取失败" };
        }
    }
}
