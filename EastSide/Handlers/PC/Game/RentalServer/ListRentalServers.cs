using System.Linq;
using EastSide.Manager;
using EastSide.Type;
using EastSide.Entities.Web.RentalGame;
using Serilog;

namespace EastSide.Handlers.PC.Game.RentalServer;

public class ListRentalServers
{
    public ListRentalServersResult Execute(int offset, int limit)
    {
        var user = UserManager.Instance.GetLastAvailableUser();
        if (user == null)
        {
            return new ListRentalServersResult { NotLogin = true };
        }

        try
        {
            var result = AppState.X19.GetRentalGameList(user.UserId, user.AccessToken, offset);
            
            var items = result.Data?.Select(item => new RentalServerItem
            {
                EntityId = item.EntityId,
                Name = string.IsNullOrEmpty(item.ServerName) ? item.Name : item.ServerName,
                PlayerCount = (int)item.PlayerCount,
                HasPassword = item.HasPassword == "1",
                McVersion = item.McVersion,
                ImageUrl = item.ImageUrl?.FirstOrDefault() ?? string.Empty
            }).ToList() ?? new();
            
            var hasMore = items.Count >= limit;
            return new ListRentalServersResult { Success = true, Items = items, HasMore = hasMore };
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "获取租赁服列表失败");
            return new ListRentalServersResult { Success = false, Message = "获取失败" };
        }
    }
}
