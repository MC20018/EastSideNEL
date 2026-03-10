using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EastSide.Core.Api;
using EastSide.Handlers.Plugin;
using EastSide.Type;

namespace EastSide.UI.Bridge;

public static class SystemHandler
{
    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetOpenFileNameW(ref OPENFILENAME lpofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OPENFILENAME
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public IntPtr lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public IntPtr lpstrFile;
        public int nMaxFile;
        public IntPtr lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }
    public static async Task<BridgeResponse> GetAnnouncement(BridgeRequest req)
    {
        var result = await OxygenApi.Instance.GetAnnouncementAsync();
        if (!result.Success)
            return BridgeResponse.Fail(req, result.Message ?? "获取公告失败");

        return BridgeResponse.Ok(req, new
        {
            title = result.Title ?? "",
            content = result.Content ?? "",
            level = result.Level ?? "info",
            updated = result.Updated ?? ""
        });
    }

    public static BridgeResponse Restart(BridgeRequest req)
    {
        try
        {
            new RestartGateway().Execute();
            return BridgeResponse.Ok(req, new { message = "正在重启..." });
        }
        catch (Exception ex)
        {
            return BridgeResponse.Fail(req, "重启失败: " + ex.Message);
        }
    }

    public static BridgeResponse SetBackdrop(BridgeRequest req)
    {
        try
        {
            var style = req.Data?.GetProperty("style").GetString() ?? "none";
            WindowEffects.Apply(style);

            var needRestart = style is "acrylic" or "mica";
            return BridgeResponse.Ok(req, new { applied = true, needRestart });
        }
        catch (Exception ex)
        {
            return BridgeResponse.Fail(req, "设置特效失败: " + ex.Message);
        }
    }

    public static BridgeResponse BrowseFile(BridgeRequest req)
    {
        try
        {
            var filter = req.Data?.GetProperty("filter").GetString() ?? "所有文件\0*.*\0";
            var title = req.Data?.GetProperty("title").GetString() ?? "选择文件";

            filter = filter.Replace("|", "\0") + "\0";

            var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
            var fileBuffer = Marshal.AllocHGlobal(520);
            Marshal.Copy(new byte[520], 0, fileBuffer, 520);

            var ofn = new OPENFILENAME
            {
                lStructSize = Marshal.SizeOf<OPENFILENAME>(),
                hwndOwner = hwnd,
                lpstrFilter = filter,
                lpstrFile = fileBuffer,
                nMaxFile = 260,
                lpstrTitle = title,
                Flags = 0x00000800 | 0x00001000,
            };

            if (GetOpenFileNameW(ref ofn))
            {
                var path = Marshal.PtrToStringUni(fileBuffer) ?? "";
                Marshal.FreeHGlobal(fileBuffer);
                return BridgeResponse.Ok(req, new { path });
            }

            Marshal.FreeHGlobal(fileBuffer);
            return BridgeResponse.Ok(req, new { path = "" });
        }
        catch (Exception ex)
        {
            return BridgeResponse.Fail(req, "打开文件对话框失败: " + ex.Message);
        }
    }

    public static BridgeResponse GetAbout(BridgeRequest req)
    {
        return BridgeResponse.Ok(req, new { version = AppState.AppVersion });
    }

    public static BridgeResponse OpenUrl(BridgeRequest req)
    {
        try
        {
            var url = req.Data?.GetProperty("url").GetString() ?? "";
            if (string.IsNullOrWhiteSpace(url)) return BridgeResponse.Fail(req, "缺少 url");
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            return BridgeResponse.Ok(req);
        }
        catch (Exception ex)
        {
            return BridgeResponse.Fail(req, "打开链接失败: " + ex.Message);
        }
    }
}
