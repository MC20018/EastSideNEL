using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Utils.Cipher;
using Serilog;

namespace EastSide.IRC;

public static class NeteaseSkinLookup
{
    const string BaseUrl = "https://x19mclobt.nie.netease.com";
    const string GameBaseUrl = "https://x19apigatewayobt.nie.netease.com";
    static readonly int[] GameTypes = [2, 8, 9, 7, 10];
    static readonly HttpClient Http = new();
    static readonly ConcurrentDictionary<string, (string SkinId, string SkinUrl, int SkinMode)?> _cache = new();

    public static async Task<(string SkinId, string SkinUrl, int SkinMode)?> LookupAsync(
        string playerName, string gameId, string userId, string userToken)
    {
        var key = $"{playerName}:{gameId}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        var targetUserId = await SearchCharacterAsync(playerName, gameId, userId, userToken);
        if (targetUserId == null) { _cache[key] = null; return null; }

        var skinInfo = await GetSkinIdAsync(targetUserId, userId, userToken);
        if (skinInfo == null) { _cache[key] = null; return null; }

        var skinUrl = await GetSkinDownloadUrlAsync(skinInfo.Value.SkinId, userId, userToken);
        if (skinUrl == null) { _cache[key] = null; return null; }

        var result = (skinInfo.Value.SkinId, skinUrl, skinInfo.Value.SkinMode);
        _cache[key] = result;
        return result;
    }

    static async Task<string?> SearchCharacterAsync(string playerName, string gameId, string userId, string userToken)
    {
        foreach (var gt in GameTypes)
        {
            var body = JsonSerializer.Serialize(new { game_id = gameId, game_type = gt, name = playerName });
            var resp = await PostAsync(GameBaseUrl, "/game-character/query/search-by-character", body, userId, userToken);
            var doc = JsonDocument.Parse(resp);
            if (doc.RootElement.GetProperty("code").GetInt32() != 0) continue;
            var entities = doc.RootElement.GetProperty("entities");
            if (entities.GetArrayLength() > 0)
                return entities[0].GetProperty("user_id").GetString();
        }
        return null;
    }

    static async Task<(string SkinId, int SkinMode)?> GetSkinIdAsync(string targetUserId, string userId, string userToken)
    {
        var body = JsonSerializer.Serialize(new { user_id = targetUserId });
        var resp = await PostAsync(GameBaseUrl, "/user-game-skin/query/search-by-type", body, userId, userToken);
        var doc = JsonDocument.Parse(resp);
        if (doc.RootElement.GetProperty("code").GetInt32() != 0) return null;
        foreach (var e in doc.RootElement.GetProperty("entities").EnumerateArray())
        {
            if (e.GetProperty("skin_type").GetInt32() == 31)
            {
                var skinId = e.GetProperty("skin_id").GetString()!;
                var skinMode = e.TryGetProperty("skin_mode", out var sm) ? sm.GetInt32() : 0;
                return (skinId, skinMode);
            }
        }
        return null;
    }

    static async Task<string?> GetSkinDownloadUrlAsync(string skinId, string userId, string userToken)
    {
        var body = JsonSerializer.Serialize(new { item_id = skinId });
        var resp = await PostAsync(BaseUrl, "/user-item-download-v2", body, userId, userToken);
        var doc = JsonDocument.Parse(resp);
        if (doc.RootElement.TryGetProperty("code", out var code) && code.GetInt32() != 0) return null;
        if (doc.RootElement.TryGetProperty("entity", out var entity) &&
            entity.TryGetProperty("sub_entities", out var subs) && subs.GetArrayLength() > 0)
            return subs[0].GetProperty("res_url").GetString();
        if (doc.RootElement.TryGetProperty("entities", out var entities) && entities.GetArrayLength() > 0)
        {
            var s = entities[0].GetProperty("sub_entities");
            if (s.GetArrayLength() > 0) return s[0].GetProperty("res_url").GetString();
        }
        return null;
    }

    static async Task<string> PostAsync(string baseUrl, string path, string body, string userId, string userToken)
    {
        var url = baseUrl + path;
        var headers = TokenUtil.ComputeHttpRequestToken(path, body, userId, userToken);
        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(body, Encoding.UTF8, "application/json");
        foreach (var kv in headers)
            req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        req.Headers.Add("X_TRACE_ID", Guid.NewGuid().ToString("N"));
        var resp = await Http.SendAsync(req);
        return await resp.Content.ReadAsStringAsync();
    }
}
