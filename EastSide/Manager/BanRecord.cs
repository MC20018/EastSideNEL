using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using EastSide.Type;
using Serilog;

namespace EastSide.Manager;

public class BanEntry
{
    public string UserId { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BanTime { get; set; }
    public DateTime? UnbanTime { get; set; }
    public bool IsPermanent { get; set; }
}

public class BanRecordManager
{
    private static readonly string FilePath = Path.Combine(AppState.DataFolder, "bans.json");
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private static readonly Lazy<BanRecordManager> _lazy = new(() => new BanRecordManager());
    public static BanRecordManager Instance => _lazy.Value;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<BanEntry> _entries = new();

    private BanRecordManager()
    {
        Load();
    }

    public static event Action<BanEntry>? OnBanAdded;

    public void AddBan(BanEntry entry)
    {
        _lock.Wait();
        try
        {
            _entries.RemoveAll(e =>
                e.UserId == entry.UserId &&
                e.ServerId == entry.ServerId &&
                string.Equals(e.RoleName, entry.RoleName, StringComparison.OrdinalIgnoreCase));
            _entries.Add(entry);
            Save();
            Log.Information("[BanRecord] 记录封禁: User={UserId}, Server={ServerId}, Role={Role}, Permanent={Perm}, UnbanTime={Unban}",
                entry.UserId, entry.ServerId, entry.RoleName, entry.IsPermanent, entry.UnbanTime);
        }
        finally { _lock.Release(); }

        try { OnBanAdded?.Invoke(entry); }
        catch (Exception ex) { Log.Debug(ex, "[BanRecord] OnBanAdded 回调异常"); }
    }

    public BanEntry? GetBanEntry(string userId, string serverId, string roleName)
    {
        _lock.Wait();
        try
        {
            var entry = _entries.FirstOrDefault(e =>
                e.UserId == userId &&
                e.ServerId == serverId &&
                string.Equals(e.RoleName, roleName, StringComparison.OrdinalIgnoreCase));
            if (entry == null) return null;
            if (!entry.IsPermanent && entry.UnbanTime != null && DateTime.Now >= entry.UnbanTime.Value)
            {
                _entries.Remove(entry);
                Save();
                return null;
            }
            return entry;
        }
        finally { _lock.Release(); }
    }

    public bool IsCurrentlyBanned(string userId, string serverId, string roleName)
    {
        _lock.Wait();
        try
        {
            var entry = _entries.FirstOrDefault(e =>
                e.UserId == userId &&
                e.ServerId == serverId &&
                string.Equals(e.RoleName, roleName, StringComparison.OrdinalIgnoreCase));
            if (entry == null) return false;
            if (entry.IsPermanent) return true;
            if (entry.UnbanTime == null) return false;
            return DateTime.Now < entry.UnbanTime.Value;
        }
        finally { _lock.Release(); }
    }

    public List<string> GetBannedRoles(string userId, string serverId)
    {
        _lock.Wait();
        try
        {
            return _entries
                .Where(e => e.UserId == userId && e.ServerId == serverId)
                .Where(e => e.IsPermanent || (e.UnbanTime != null && DateTime.Now < e.UnbanTime.Value))
                .Select(e => e.RoleName)
                .ToList();
        }
        finally { _lock.Release(); }
    }

    public void RemoveExpired()
    {
        _lock.Wait();
        try
        {
            var before = _entries.Count;
            _entries.RemoveAll(e => !e.IsPermanent && e.UnbanTime != null && DateTime.Now >= e.UnbanTime.Value);
            if (_entries.Count != before) Save();
        }
        finally { _lock.Release(); }
    }

    public List<BanEntry> GetAll()
    {
        _lock.Wait();
        try { return _entries.ToList(); }
        finally { _lock.Release(); }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return;
            var json = File.ReadAllText(FilePath);
            _entries = JsonSerializer.Deserialize<List<BanEntry>>(json) ?? new();
            Log.Information("[BanRecord] 加载 {Count} 条封禁记录", _entries.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BanRecord] 加载封禁记录失败");
            _entries = new();
        }
    }

    private void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(_entries, JsonOpts));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BanRecord] 保存封禁记录失败");
        }
    }
}
