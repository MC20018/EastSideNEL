using System;
using System.IO;
using EastSide.Manager;
using EastSide.UI.Bridge;
using EastSide.Utils;
using Serilog;
using Serilog.Events;

namespace EastSide.UI;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var baseDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
        Directory.SetCurrentDirectory(baseDir);

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            string info = ex?.ToString() ?? e.ExceptionObject?.ToString() ?? "Unknown error";
            File.AppendAllText("crash.log", $"[Unhandled] {info}\n");
        };

        ConfigureLogger();
        Log.Information("EastSide.UI 启动中...");

        NotificationHelper.OnNotify += (message, level) =>
        {
            var levelStr = level switch
            {
                NotifyLevel.Success => "success",
                NotifyLevel.Warning => "warning",
                NotifyLevel.Error => "error",
                _ => "info"
            };
            AppWindow.PushNotification(message, levelStr);
        };

        BanRecordManager.OnBanAdded += entry =>
        {
            AppWindow.PushEvent("ban:updated", new
            {
                serverId = entry.ServerId,
                roleName = entry.RoleName,
                permanent = entry.IsPermanent,
                unbanTime = entry.UnbanTime?.ToString("o")
            });
        };

        Backend.Initialize();

        AppWindow.Run();
    }

    private static void ConfigureLogger()
    {
        var baseDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
        var logDir = Path.Combine(baseDir, "logs");
        Directory.CreateDirectory(logDir);
        var fileName = DateTime.Now.ToString("yyyy-MM-dd-HHmm-ss") + ".log";
        var filePath = Path.Combine(logDir, fileName);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Information)
            .WriteTo.Console()
            .WriteTo.File(filePath)
            .CreateLogger();
    }
}
