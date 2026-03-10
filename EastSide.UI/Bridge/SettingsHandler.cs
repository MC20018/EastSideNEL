using System.Text.Json;
using EastSide.Manager;
using EastSide.Type;

namespace EastSide.UI.Bridge;

public static class SettingsHandler
{
    public static BridgeResponse GetSettings(BridgeRequest req)
    {
        var s = SettingManager.Instance.Get();
        return BridgeResponse.Ok(req, new
        {
            themeMode = s.ThemeMode,
            themeColor = s.ThemeColor,
            backdrop = s.Backdrop,
            customBackgroundPath = s.CustomBackgroundPath,
            autoCopyIpOnStart = s.AutoCopyIpOnStart,
            debug = s.Debug,
            autoDisconnectOnBan = s.AutoDisconnectOnBan,
            socks5Enabled = s.Socks5Enabled,
            socks5Address = s.Socks5Address,
            socks5Port = s.Socks5Port,
            socks5Username = s.Socks5Username,
            socks5Password = s.Socks5Password,
        });
    }

    public static BridgeResponse UpdateSettings(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");

        var s = SettingManager.Instance.Get();
        var data = req.Data.Value;

        if (data.TryGetProperty("themeMode", out var v1)) s.ThemeMode = v1.GetString() ?? "system";
        if (data.TryGetProperty("themeColor", out var v2)) s.ThemeColor = v2.GetString() ?? "#0078D4";
        if (data.TryGetProperty("backdrop", out var v3)) s.Backdrop = v3.GetString() ?? "none";
        if (data.TryGetProperty("customBackgroundPath", out var v3b)) s.CustomBackgroundPath = v3b.GetString() ?? "";
        if (data.TryGetProperty("autoCopyIpOnStart", out var v4)) s.AutoCopyIpOnStart = v4.GetBoolean();
        if (data.TryGetProperty("debug", out var v5)) s.Debug = v5.GetBoolean();
        if (data.TryGetProperty("autoDisconnectOnBan", out var v6)) s.AutoDisconnectOnBan = v6.GetString() ?? "none";
        if (data.TryGetProperty("socks5Enabled", out var v7)) s.Socks5Enabled = v7.GetBoolean();
        if (data.TryGetProperty("socks5Address", out var v8)) s.Socks5Address = v8.GetString() ?? "";
        if (data.TryGetProperty("socks5Port", out var v9)) s.Socks5Port = v9.GetInt32();
        if (data.TryGetProperty("socks5Username", out var v10)) s.Socks5Username = v10.GetString() ?? "";
        if (data.TryGetProperty("socks5Password", out var v11)) s.Socks5Password = v11.GetString() ?? "";

        SettingManager.Instance.SaveToDisk();
        return BridgeResponse.Ok(req, new { message = "设置已保存" });
    }
}
