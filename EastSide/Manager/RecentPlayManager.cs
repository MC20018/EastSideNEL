using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using EastSide.Type;
using Serilog;

namespace EastSide.Manager;

public class RecentEntry
{
    public string ServerId { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "network" | "rental"
    public DateTime PlayTime { get; set; }
    public string? McVersion { get; set; }
    public bool HasPassword { get; set; }
}

public class RecentPlayManager
{
    private static readonly string FilePath = Path.Combine(AppState.DataFolder, "recent.json");
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private static readonly Lazy<RecentPlayManager> _lazy = new(() => new RecentPlayManager());
    public static RecentPlayManager Instance => _lazy.Value;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<RecentEntry> _entries = new();
    private const int MaxEntries = 20;

    private RecentPlayManager()
    {
        Load();
    }

    public void Add(RecentEntry entry)
    {
        _lock.Wait();
        try
        {
            _entries.RemoveAll(e => e.ServerId == entry.ServerId && e.Type == entry.Type);
            _entries.Insert(0, entry);
            if (_entries.Count > MaxEntries)
                _entries = _entries.Take(MaxEntries).ToList();
            Save();
            Log.Information("[RecentPlay] 记录游玩: Server={ServerId}, Name={Name}, Type={Type}",
                entry.ServerId, entry.ServerName, entry.Type);
        }
        finally { _lock.Release(); }
    }

    public List<RecentEntry> GetAll()
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
            _entries = JsonSerializer.Deserialize<List<RecentEntry>>(json) ?? new();
            Log.Information("[RecentPlay] 加载 {Count} 条最近游玩记录", _entries.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[RecentPlay] 加载最近游玩记录失败");
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
            Log.Error(ex, "[RecentPlay] 保存最近游玩记录失败");
        }
    }
}
