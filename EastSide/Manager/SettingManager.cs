using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using Serilog;
using EastSide.Type;

namespace EastSide.Manager;

public class SettingManager
{
    private const string DataFolder = AppState.DataFolder;
    private static readonly string SettingsFilePath = Path.Combine(DataFolder, "setting.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly Lazy<SettingManager> _lazy = new(() => new SettingManager());
    public static SettingManager Instance => _lazy.Value;

    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private SettingData _settings = new();

    private SettingManager()
    {
        EnsureDataFolder();
        MigrateOldSettings();
        ReadFromDisk();
        _settings.PropertyChanged += (_, _) => SaveToDisk();
    }

    private static void EnsureDataFolder()
    {
        if (!Directory.Exists(DataFolder))
            Directory.CreateDirectory(DataFolder);
    }

    private static void MigrateOldSettings()
    {
        const string oldPath = "setting.json";
        if (File.Exists(oldPath) && !File.Exists(SettingsFilePath))
        {
            try
            {
                File.Move(oldPath, SettingsFilePath);
                Log.Information("已迁移旧设置文件到 data 目录");
            }
            catch (Exception ex) { Log.Warning(ex, "迁移旧设置文件失败"); }
        }
    }

    public static string CopyBackgroundToData(string sourcePath)
    {
        EnsureDataFolder();
        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(DataFolder, fileName);

        if (File.Exists(destPath))
        {
            var name = Path.GetFileNameWithoutExtension(sourcePath);
            var ext = Path.GetExtension(sourcePath);
            destPath = Path.Combine(DataFolder, $"{name}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
        }

        File.Copy(sourcePath, destPath, false);
        Log.Information("已复制背景文件到: {Path}", destPath);
        return destPath;
    }

    public static string CopyMusicToData(string sourcePath)
    {
        EnsureDataFolder();
        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(DataFolder, fileName);

        if (File.Exists(destPath))
        {
            var name = Path.GetFileNameWithoutExtension(sourcePath);
            var ext = Path.GetExtension(sourcePath);
            destPath = Path.Combine(DataFolder, $"{name}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
        }

        File.Copy(sourcePath, destPath, false);
        Log.Information("已复制音乐文件到: {Path}", destPath);
        return destPath;
    }

    public SettingData Get() => _settings;

    public void ReadFromDisk()
    {
        try
        {
            if (!File.Exists(SettingsFilePath)) { _settings = new(); return; }
            var obj = JsonSerializer.Deserialize<SettingData>(File.ReadAllText(SettingsFilePath)) ?? new();
            _settings = obj;
            _settings.PropertyChanged += (_, _) => SaveToDisk();
            Log.Information("设置已加载");
        }
        catch (Exception ex) { Log.Error(ex, "加载设置失败"); _settings = new(); }
    }

    public void SaveToDisk()
    {
        if (!_saveLock.Wait(0)) return;
        try { File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(_settings, JsonOptions)); }
        catch (Exception ex) { Log.Error(ex, "保存设置失败"); }
        finally { _saveLock.Release(); }
    }
}
