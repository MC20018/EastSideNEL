using Serilog;

namespace EastSide.UI.Bridge;

public static class LogHandler
{
    public static BridgeResponse Info(BridgeRequest req)
    {
        var msg = req.Data?.GetProperty("message").GetString() ?? "";
        Log.Information("[Frontend] {Message}", msg);
        return BridgeResponse.Ok(req);
    }

    public static BridgeResponse Error(BridgeRequest req)
    {
        var msg = req.Data?.GetProperty("message").GetString() ?? "";
        Log.Error("[Frontend] {Message}", msg);
        return BridgeResponse.Ok(req);
    }
}
