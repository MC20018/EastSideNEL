using System;
using System.Collections.Generic;
using System.Linq;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Entity.WFPLauncher.NetGame;
using EastSide.Type;
using EastSide.Manager;
using EastSide.Utils;
using Serilog;
using EastSide.Entities.Web.NetGame;

namespace EastSide.Handlers.PC.Game.NetGame;

public class SearchServers
{
    public ListServersResult Execute(string keyword, int offset, int pageSize)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null)
        {
            return new ListServersResult { NotLogin = true };
        }
        try
        {
            var all = AppState.X19.GetAvailableNetGames(last.UserId, last.AccessToken, 0, 100);
            var data = all.Data?.ToList() ?? new List<EntityNetGameItem>();

            var q = string.IsNullOrWhiteSpace(keyword)
                ? data
                : data.Where(s => s.Name.IndexOf(keyword!, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            var pageItems = q.Skip(offset).Take(pageSize).ToList();

            if (pageItems.Count > 0)
            {
                var entityIds = pageItems.Select(s => s.EntityId).ToArray();
                var queryResult = AppState.X19.QueryNetGameItemByIds(last.UserId, last.AccessToken, entityIds);

                var items = new List<ServerItem>();
                for (int i = 0; i < pageItems.Count; i++)
                {
                    var server = pageItems[i];
                    var imageUrl = i < queryResult.Data.Length ? queryResult.Data[i].TitleImageUrl : string.Empty;

                    items.Add(new ServerItem
                    {
                        EntityId = server.EntityId,
                        Name = server.Name,
                        OnlineCount = server.OnlineCount,
                        ImageUrl = imageUrl
                    });
                }

                var hasMore = offset + pageSize < q.Count;
                return new ListServersResult { Success = true, Items = items, HasMore = hasMore };
            }
            else
            {
                return new ListServersResult { Success = true, Items = new List<ServerItem>(), HasMore = false };
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "搜索服务器失败");
            return new ListServersResult { Success = false, Message = "搜索失败" };
        }
    }
}
