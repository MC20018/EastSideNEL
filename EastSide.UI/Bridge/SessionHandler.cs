using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Handlers.PC;
using EastSide.Handlers.Game.NetServer;
using EastSide.Manager;
using Serilog;

namespace EastSide.UI.Bridge;

public static class SessionHandler
{
    public static async Task<BridgeResponse> ListSessions(BridgeRequest req)
    {
        try
        {
            var result = await Task.Run(() => new QueryGameSession().Execute());
            var typeVal = result.GetType().GetProperty("type")?.GetValue(result) as string;
            if (!string.Equals(typeVal, "query_game_session", StringComparison.OrdinalIgnoreCase))
                return BridgeResponse.Ok(req, new { sessions = Array.Empty<object>() });

            var items = result.GetType().GetProperty("items")?.GetValue(result)
                as IEnumerable<EastSide.Entities.Web.NEL.EntityQueryGameSessions>;

            var sessions = (items ?? Enumerable.Empty<EastSide.Entities.Web.NEL.EntityQueryGameSessions>())
                .Select(s => new
                {
                    id = s.Id,
                    serverName = s.ServerName,
                    characterName = s.CharacterName,
                    type = s.Type,
                    statusText = s.StatusText,
                    localAddress = s.LocalAddress,
                    identifier = s.Guid
                }).ToList();

            return BridgeResponse.Ok(req, new { sessions });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取游戏会话失败");
            return BridgeResponse.Fail(req, "获取游戏会话失败");
        }
    }

    public static async Task<BridgeResponse> Shutdown(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var identifier = req.Data.Value.GetProperty("identifier").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(identifier))
            return BridgeResponse.Fail(req, "缺少 identifier");

        try
        {
            await Task.Run(() => new ShutdownGame().Execute(new[] { identifier }));
            return BridgeResponse.Ok(req, new { message = "已关闭" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "关闭游戏通道失败");
            return BridgeResponse.Fail(req, "关闭失败: " + ex.Message);
        }
    }
}
