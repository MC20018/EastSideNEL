using System;
using System.Linq;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Protocol;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Game;
using EastSide.Type;
using EastSide.Entities.Web.NetGame;
using EastSide.Manager;
using Serilog;

namespace EastSide.Handlers.PC.Game.NetGame;

public class DeleteRoleNamed
{
    public async Task<ServerRolesResult> Execute(string serverId, string characterName)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new ServerRolesResult { NotLogin = true };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(characterName))
            return new ServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            var version = await WPFLauncher.GetLatestVersionAsync();
            var protocol = new GameProtocol(version);
            protocol.DeleteCharacter(last.UserId, last.AccessToken, last.UserId);
            Entities<EntityGameCharacter> entities = AppState.X19.QueryNetGameCharacters(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new ServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除角色失败: serverId={ServerId}, name={Name}", serverId, characterName);
            return new ServerRolesResult { Success = false, Message = ex.Message };
        }
    }

    public async Task<ServerRolesResult> ExecuteForAccount(string accountId, string serverId, string characterName)
    {
        if (string.IsNullOrWhiteSpace(accountId)) return new ServerRolesResult { Success = false, Message = "参数错误" };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(characterName))
            return new ServerRolesResult { Success = false, Message = "参数错误" };
        try
        {
            var u = UserManager.Instance.GetAvailableUser(accountId);
            if (u == null) return new ServerRolesResult { NotLogin = true };
            var version = await WPFLauncher.GetLatestVersionAsync();
            var protocol = new GameProtocol(version);
            protocol.DeleteCharacter(u.UserId, u.AccessToken, u.UserId);
            Entities<EntityGameCharacter> entities = AppState.X19.QueryNetGameCharacters(u.UserId, u.AccessToken, serverId);
            var items = entities.Data.Select(r => new RoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new ServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除角色失败: serverId={ServerId}, name={Name}", serverId, characterName);
            return new ServerRolesResult { Success = false, Message = ex.Message };
        }
    }
}
