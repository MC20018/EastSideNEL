using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Notification
{
    public static void Send(string title, string message)
    {
        string os = OSDetect.GetOS();

        if (os == "Windows")
            SendWindowsNotification(title, message);
        else if (os == "MacOS")
            SendMacNotification(title, message);
        else if (os == "Linux")
            SendLinuxNotification(title, message);
    }

    #region Windows - shell32.dll P/Invoke

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

    [DllImport("user32.dll")]
    static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    const uint NIM_ADD = 0x00;
    const uint NIM_MODIFY = 0x01;
    const uint NIM_DELETE = 0x02;

    const uint NIF_ICON = 0x02;
    const uint NIF_TIP = 0x04;
    const uint NIF_INFO = 0x10;

    const uint NIIF_INFO = 0x01;

    static IntPtr GetNotifyIcon()
    {
        string exe = Process.GetCurrentProcess().MainModule.FileName;
        IntPtr icon = ExtractIcon(IntPtr.Zero, exe, 0);
        if (icon != IntPtr.Zero && icon != (IntPtr)1)
            return icon;

        icon = ExtractIcon(IntPtr.Zero, "shell32.dll", 277);
        if (icon != IntPtr.Zero && icon != (IntPtr)1)
            return icon;

        return ExtractIcon(IntPtr.Zero, "shell32.dll", 0);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct NOTIFYICONDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public IntPtr hBalloonIcon;
    }

    static void SendWindowsNotification(string title, string message)
    {
        var nid = new NOTIFYICONDATA();
        nid.cbSize = (uint)Marshal.SizeOf(nid);
        nid.hWnd = GetConsoleWindow();
        nid.uID = 9999;
        nid.uFlags = NIF_ICON | NIF_TIP | NIF_INFO;
        nid.hIcon = GetNotifyIcon();
        nid.szTip = title;
        nid.szInfoTitle = title;
        nid.szInfo = message;
        nid.dwInfoFlags = NIIF_INFO;

        Shell_NotifyIcon(NIM_ADD, ref nid);

        Task.Delay(5000).ContinueWith(_ =>
        {
            Shell_NotifyIcon(NIM_DELETE, ref nid);
            if (nid.hIcon != IntPtr.Zero)
                DestroyIcon(nid.hIcon);
        });
    }

    #endregion

    #region MacOS - osascript

    static void SendMacNotification(string title, string message)
    {
        try
        {
            var p = new Process();
            p.StartInfo.FileName = "osascript";
            p.StartInfo.ArgumentList.Add("-e");
            p.StartInfo.ArgumentList.Add($"display notification \"{message}\" with title \"{title}\"");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }
        catch { }
    }

    #endregion

    #region Linux - notify-send

    static void SendLinuxNotification(string title, string message)
    {
        try
        {
            var p = new Process();
            p.StartInfo.FileName = "notify-send";
            p.StartInfo.ArgumentList.Add(title);
            p.StartInfo.ArgumentList.Add(message);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }
        catch { }
    }

    #endregion
}
