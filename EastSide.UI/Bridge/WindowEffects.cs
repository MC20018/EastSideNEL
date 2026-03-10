using System;
using System.Runtime.InteropServices;
using EastSide.Manager;
using Serilog;

namespace EastSide.UI.Bridge;

public static class WindowEffects
{
    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public int Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    private const int WCA_ACCENT_POLICY = 19;
    private const int ACCENT_DISABLED = 0;
    private const int ACCENT_ENABLE_BLURBEHIND = 3;
    private const int ACCENT_ENABLE_ACRYLICBLURBEHIND = 4;

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int Left, Right, Top, Bottom;
    }

    private static string _currentStyle = "none";

    public static void Apply(string style)
    {
        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        _currentStyle = style ?? "none";

        try
        {
            switch (_currentStyle)
            {
                case "acrylic":
                    ExtendFrame(hwnd);
                    SetAccent(hwnd, ACCENT_ENABLE_ACRYLICBLURBEHIND, 0x01000000);
                    Log.Information("已应用亚克力效果");
                    break;

                case "mica":
                    ExtendFrame(hwnd);
                    SetAccent(hwnd, ACCENT_ENABLE_BLURBEHIND, 0x01000000);
                    Log.Information("已应用 Mica(blur) 效果");
                    break;

                default:
                    SetAccent(hwnd, ACCENT_DISABLED, 0);
                    _currentStyle = "none";
                    Log.Information("已关闭窗口特效");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "应用窗口特效失败: {Style}", style);
        }
    }

    public static void OnDragStart()
    {
        if (_currentStyle != "acrylic") return;

        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        try
        {
            SetAccent(hwnd, ACCENT_ENABLE_BLURBEHIND, 0x01000000);
        }
        catch {  }
    }

    public static void OnDragEnd()
    {
        if (_currentStyle != "acrylic") return;

        var hwnd = AppWindow.Instance?.WindowHandle ?? IntPtr.Zero;
        if (hwnd == IntPtr.Zero) return;

        try
        {
            SetAccent(hwnd, ACCENT_ENABLE_ACRYLICBLURBEHIND, 0x01000000);
        }
        catch {  }
    }

    private static void SetAccent(IntPtr hwnd, int accentState, uint gradientColor)
    {
        var accent = new AccentPolicy
        {
            AccentState = accentState,
            AccentFlags = 2,
            GradientColor = gradientColor,
            AnimationId = 0
        };

        var size = Marshal.SizeOf(accent);
        var ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(accent, ptr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WCA_ACCENT_POLICY,
                Data = ptr,
                SizeOfData = size
            };
            SetWindowCompositionAttribute(hwnd, ref data);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static void ExtendFrame(IntPtr hwnd)
    {
        var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
        WindowHandler.ApplyRoundedCorners();
    }
}
