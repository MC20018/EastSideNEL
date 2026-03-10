using System;
using System.Linq;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Entity.WFPLauncher.NetGame;
using EastSide.Type;
using EastSide.Entities.Web.NetGame;
using Serilog;
using EastSide.Manager;

namespace EastSide.Handlers.PC.Game.NetGame;

public class CreateRoleNamed
{
    public ServerRolesResult Execute(string serverId, string name)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new ServerRolesResult { NotLogin = true };
        if (string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(name))
        {
            return new ServerRolesResult { Success = false, Message = "参数错误" };
        }
        try
        {
            AppState.X19.CreateCharacter(last.UserId, last.AccessToken, serverId, name);
            if (SettingManager.Instance.Get().Debug) Log.Information("角色创建成功: serverId={ServerId}, name={Name}", serverId, name);
            Codexus.Cipher.Entities.Entities<EntityGameCharacter> entities = AppState.X19.QueryNetGameCharacters(last.UserId, last.AccessToken, serverId);
            var items = entities.Data.Select(r => new RoleItem { Id = r.Name, Name = r.Name }).ToList();
            return new ServerRolesResult { Success = true, ServerId = serverId, Items = items };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "角色创建失败: serverId={ServerId}, name={Name}", serverId, name);
            return new ServerRolesResult { Success = false, Message = ex.Message };
        }
    }
}
