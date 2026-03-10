using System;

namespace EastSide.Utils;

public enum NotifyLevel
{
    Info,
    Success,
    Warning,
    Error
}

public static class NotificationHelper
{
    public static event Action<string, NotifyLevel>? OnNotify;

    public static void ShowSuccess(string message) => OnNotify?.Invoke(message, NotifyLevel.Success);
    public static void ShowError(string message) => OnNotify?.Invoke(message, NotifyLevel.Error);
    public static void ShowWarning(string message) => OnNotify?.Invoke(message, NotifyLevel.Warning);
    public static void ShowInfo(string message) => OnNotify?.Invoke(message, NotifyLevel.Info);
}
