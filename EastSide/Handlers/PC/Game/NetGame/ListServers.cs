using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Entity.WFPLauncher.NetGame;
using Serilog;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Entities.Web.NetGame;

namespace EastSide.Handlers.PC.Game.NetGame;

public class ListServers
{
    public ListServersResult Execute(int offset, int pageSize)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new ListServersResult { NotLogin = true };
        try
        {
            var servers = AppState.X19.GetAvailableNetGames(last.UserId, last.AccessToken, offset, pageSize);
            var data = servers.Data?.ToList() ?? new List<EntityNetGameItem>();
            
            if (data.Count == 0)
            {
                return new ListServersResult { Success = true, Items = new List<ServerItem>(), HasMore = false };
            }

            var entityIds = data.Select(s => s.EntityId).ToArray();
            var queryResult = AppState.X19.QueryNetGameItemByIds(last.UserId, last.AccessToken, entityIds);

            var items = new List<ServerItem>();
            for (int i = 0; i < data.Count; i++)
            {
                var server = data[i];
                var imageUrl = i < queryResult.Data.Length ? queryResult.Data[i].TitleImageUrl : string.Empty;
                
                items.Add(new ServerItem 
                { 
                    EntityId = server.EntityId, 
                    Name = server.Name, 
                    OnlineCount = server.OnlineCount,
                    ImageUrl = imageUrl
                });
            }

            return new ListServersResult { Success = true, Items = items, HasMore = data.Count >= pageSize };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取服务器列表失败");
            return new ListServersResult { Success = false, Message = "获取失败" };
        }
    }
}
