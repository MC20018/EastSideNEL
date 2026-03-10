using System;
using System.Runtime.InteropServices;
using Serilog;

namespace EastSide.UI.Bridge;

public static class WindowHandler
{
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_ROUND = 2;

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

    [DllImport("user32.dll")]
    private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int Left, Top, Right, Bottom; }

    private static bool _useRegionFallback;
    private const int CORNER_RADIUS = 12;

    public static void ApplyRoundedCorners()
    {
        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        try
        {
            int preference = DWMWCP_ROUND;
            int hr = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
            if (hr == 0)
            {
                return;
            }
        }
        catch {  }

        _useRegionFallback = true;
        ApplyRoundedRegion(hwnd);
    }

    public static void OnWindowSizeChanged()
    {
        if (!_useRegionFallback) return;

        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        var w = AppWindow.Instance;
        if (w != null && w.Maximized)
        {
            SetWindowRgn(hwnd, IntPtr.Zero, true);
        }
        else
        {
            ApplyRoundedRegion(hwnd);
        }
    }

    private static void ApplyRoundedRegion(IntPtr hwnd)
    {
        if (!GetWindowRect(hwnd, out var rect)) return;

        int w = rect.Right - rect.Left;
        int h = rect.Bottom - rect.Top;
        if (w <= 0 || h <= 0) return;

        var rgn = CreateRoundRectRgn(0, 0, w + 1, h + 1, CORNER_RADIUS, CORNER_RADIUS);
        SetWindowRgn(hwnd, rgn, true);
    }

    public static BridgeResponse StartDrag(BridgeRequest req)
    {
        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return BridgeResponse.Fail(req, "窗口未初始化");

        WindowEffects.OnDragStart();

        ReleaseCapture();
        SendMessage(hwnd, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);

        WindowEffects.OnDragEnd();

        return BridgeResponse.Ok(req);
    }

    public static BridgeResponse Minimize(BridgeRequest req)
    {
        AppWindow.Instance?.SetMinimized(true);
        return BridgeResponse.Ok(req);
    }

    public static BridgeResponse Maximize(BridgeRequest req)
    {
        var w = AppWindow.Instance;
        if (w == null) return BridgeResponse.Fail(req, "窗口未初始化");

        if (w.Maximized)
            w.SetMaximized(false);
        else
            w.SetMaximized(true);

        OnWindowSizeChanged();

        return BridgeResponse.Ok(req);
    }

    public static BridgeResponse Close(BridgeRequest req)
    {
        AppWindow.Instance?.Close();
        return BridgeResponse.Ok(req);
    }
}
