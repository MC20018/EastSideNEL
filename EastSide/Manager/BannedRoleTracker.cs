using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codexus.Interceptors;
using Codexus.Development.SDK.Entities;
using EastSide.Entities.Web.NetGame;
using EastSide.Handlers.Game.NetServer;
using EastSide.Handlers.PC.Game.NetGame;
using EastSide.Type;
using EastSide.Utils;
using Serilog;

namespace EastSide.Manager;

public static class BannedRoleTracker
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<string>>> _bannedRoles = new();

    public static void MarkBanned(string userId, string serverId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
            return;

        var userDict = _bannedRoles.GetOrAdd(userId, _ => new ConcurrentDictionary<string, HashSet<string>>());
        var serverSet = userDict.GetOrAdd(serverId, _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        lock (serverSet)
        {
            serverSet.Add(roleName);
        }

        Log.Information("[BannedRoleTracker] 标记封禁: UserId={UserId}, ServerId={ServerId}, Role={Role}", userId, serverId, roleName);
    }

    public static void UnmarkBanned(string userId, string serverId, string roleName)
    {
        if (!_bannedRoles.TryGetValue(userId, out var userDict)) return;
        if (!userDict.TryGetValue(serverId, out var serverSet)) return;
        lock (serverSet) { serverSet.Remove(roleName); }
    }

    public static bool IsBanned(string userId, string serverId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(roleName))
            return false;

        var banEntry = BanRecordManager.Instance.GetBanEntry(userId, serverId, roleName);
        if (banEntry != null)
        {
            if (banEntry.IsPermanent) return true;
            if (banEntry.UnbanTime != null && DateTime.Now < banEntry.UnbanTime.Value) return true;
            // 已过期，顺便清理内存标记
            UnmarkBanned(userId, serverId, roleName);
            return false;
        }

        if (!_bannedRoles.TryGetValue(userId, out var userDict))
            return false;

        if (!userDict.TryGetValue(serverId, out var serverSet))
            return false;

        lock (serverSet)
        {
            return serverSet.Contains(roleName);
        }
    }

    public static List<string> GetAvailableRoles(string userId, string accessToken, string serverId)
    {
        try
        {
            var roles = AppState.X19.QueryNetGameCharacters(userId, accessToken, serverId);
            var allRoles = roles.Data?.Select(r => r.Name).ToList() ?? new List<string>();

            // 清理过期的持久化记录
            BanRecordManager.Instance.RemoveExpired();
            var persistBanned = BanRecordManager.Instance.GetBannedRoles(userId, serverId);

            // 清理内存中已过期的标记
            foreach (var role in allRoles)
            {
                var entry = BanRecordManager.Instance.GetBanEntry(userId, serverId, role);
                if (entry != null && !entry.IsPermanent && entry.UnbanTime != null && DateTime.Now >= entry.UnbanTime.Value)
                    UnmarkBanned(userId, serverId, role);
            }

            if (!_bannedRoles.TryGetValue(userId, out var userDict))
                return allRoles.Where(r => !persistBanned.Contains(r)).ToList();

            if (!userDict.TryGetValue(serverId, out var serverSet))
                return allRoles.Where(r => !persistBanned.Contains(r)).ToList();

            lock (serverSet)
            {
                return allRoles.Where(r => !serverSet.Contains(r) && !persistBanned.Contains(r)).ToList();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BannedRoleTracker] 获取可用角色失败");
            return new List<string>();
        }
    }

    public static void ClearUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;

        _bannedRoles.TryRemove(userId, out _);
        Log.Information("[BannedRoleTracker] 清除用户记录: UserId={UserId}", userId);
    }

    public static void ClearAll()
    {
        _bannedRoles.Clear();
        Log.Information("[BannedRoleTracker] 清除所有记录");
    }

    public static async Task<bool> TrySwitchToAnotherRole(
        string userId, 
        string accessToken, 
        string serverId, 
        string serverName,
        string currentRole,
        EntitySocks5? socks5)
    {
        try
        {
            MarkBanned(userId, serverId, currentRole);

            var availableRoles = GetAvailableRoles(userId, accessToken, serverId);
            
            if (availableRoles.Count == 0)
            {
                Log.Warning("[BannedRoleTracker] 没有可用角色了: UserId={UserId}, ServerId={ServerId}", userId, serverId);
                NotificationHelper.ShowWarning("此账号在该服务器没有其他可用角色");
                return false;
            }

            var nextRole = availableRoles.Last();
            Log.Information("[BannedRoleTracker] 切换到角色: {Role}", nextRole);

            var joinGame = new JoinGame();
            var request = new EntityJoinGame
            {
                ServerId = serverId,
                ServerName = serverName,
                Role = nextRole,
                Socks5 = socks5 ?? new EntitySocks5()
            };

            var result = await joinGame.Execute(request);
            if (result.Success)
            {
                NotificationHelper.ShowSuccess($"已切换到角色: {nextRole}");
                return true;
            }
            else
            {
                Log.Warning("[BannedRoleTracker] 切换角色失败: {Message}", result.Message);
                NotificationHelper.ShowError($"切换角色失败: {result.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BannedRoleTracker] 切换角色异常");
            NotificationHelper.ShowError("切换角色时发生错误");
            return false;
        }
    }
}
