using EastSide.Manager;
using System.Linq;

namespace EastSide.UI.Bridge;

public static class OverviewHandler
{
    public static BridgeResponse GetRecent(BridgeRequest req)
    {
        var items = RecentPlayManager.Instance.GetAll().Select(e => new
        {
            serverId = e.ServerId,
            serverName = e.ServerName,
            type = e.Type,
            playTime = e.PlayTime.ToString("o"),
            mcVersion = e.McVersion,
            hasPassword = e.HasPassword
        }).ToList();

        return BridgeResponse.Ok(req, new { items });
    }
}
