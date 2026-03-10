using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace EastSide.Core.Cache;

public static class ImageCacheManager
{
    private static readonly ConcurrentDictionary<string, byte[]> _memoryCache = new();
    private static readonly ConcurrentDictionary<string, Task<byte[]?>> _pendingDownloads = new();
    private static readonly SemaphoreSlim _downloadSemaphore = new(3);
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private static readonly string _imageCacheDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EastSide", "Cache", "Images");

    private static readonly TimeSpan _defaultMaxAge = TimeSpan.FromDays(7);

    static ImageCacheManager()
    {
        Directory.CreateDirectory(_imageCacheDir);
    }

    public static async Task<byte[]?> GetAsync(string url)
    {
        var key = GetCacheKey(url);

        if (_memoryCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var filePath = GetCacheFilePath(key);
        if (File.Exists(filePath))
        {
            var fileInfo = new FileInfo(filePath);
            if (DateTime.UtcNow - fileInfo.LastWriteTimeUtc < _defaultMaxAge)
            {
                var data = await File.ReadAllBytesAsync(filePath);
                _memoryCache[key] = data;
                return data;
            }
            try { File.Delete(filePath); } catch (Exception ex) { Log.Debug(ex, "删除过期缓存文件失败"); }
        }

        return null;
    }

    public static async Task<byte[]?> GetOrDownloadAsync(string url)
    {
        var cached = await GetAsync(url);
        if (cached != null)
        {
            return cached;
        }

        return await _pendingDownloads.GetOrAdd(url, DownloadAndCacheAsync);
    }

    public static async Task<byte[]?> DownloadAndCacheAsync(string url)
    {
        await _downloadSemaphore.WaitAsync();
        try
        {
            var cached = await GetAsync(url);
            if (cached != null) return cached;

            var data = await _httpClient.GetByteArrayAsync(url);

            if (data != null && data.Length > 0)
            {
                await SetAsync(url, data);
                return data;
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "下载图片失败: {Url}", url);
        }
        finally
        {
            _downloadSemaphore.Release();
            _pendingDownloads.TryRemove(url, out _);
        }

        return null;
    }

    public static async Task SetAsync(string url, byte[] data)
    {
        var key = GetCacheKey(url);
        _memoryCache[key] = data;

        try
        {
            var filePath = GetCacheFilePath(key);
            await File.WriteAllBytesAsync(filePath, data);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "保存图片缓存失败: {Url}", url);
        }
    }

    public static void ClearAll()
    {
        _memoryCache.Clear();

        try
        {
            if (Directory.Exists(_imageCacheDir))
            {
                foreach (var file in Directory.GetFiles(_imageCacheDir, "*.*"))
                {
                    try { File.Delete(file); } catch (Exception ex) { Log.Debug(ex, "删除缓存文件失败"); }
                }
            }
        }
        catch (Exception ex) { Log.Debug(ex, "清除图片缓存目录失败"); }
    }

    public static string GetCacheFilePath(string url)
    {
        var key = GetCacheKey(url);
        return Path.Combine(_imageCacheDir, key);
    }

    private static string GetCacheKey(string url)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(url));
        return Convert.ToHexString(hash);
    }
}
