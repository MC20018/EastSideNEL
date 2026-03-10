using System;
using System.Text.RegularExpressions;
using EastSide.Manager;
using Serilog;

namespace EastSide.Utils;

public static class BanMessageParser
{
    private static readonly Regex DurationRegex = new(
        @"已被封禁\s*(\d+)\s*(秒|分钟|小时|天)", RegexOptions.Compiled);

    private static readonly Regex PermanentRegex = new(
        @"已被永久封禁|永久封禁", RegexOptions.Compiled);

    private static readonly Regex RemainingRegex = new(
        @"剩余时间:\s*(\d+)\s*(秒|分钟|小时|天)", RegexOptions.Compiled);

    public static BanEntry? Parse(string message, string userId, string serverId, string roleName)
    {
        if (string.IsNullOrEmpty(message)) return null;

        var now = DateTime.Now;

        if (PermanentRegex.IsMatch(message))
        {
            Log.Information("[BanParser] 检测到永久封禁");
            return new BanEntry
            {
                UserId = userId, ServerId = serverId, RoleName = roleName,
                Reason = message, BanTime = now,
                UnbanTime = null, IsPermanent = true
            };
        }

        var match = DurationRegex.Match(message);
        if (!match.Success) match = RemainingRegex.Match(message);

        if (match.Success)
        {
            var amount = int.Parse(match.Groups[1].Value);
            var unit = match.Groups[2].Value;
            var duration = unit switch
            {
                "秒" => TimeSpan.FromSeconds(amount),
                "分钟" => TimeSpan.FromMinutes(amount),
                "小时" => TimeSpan.FromHours(amount),
                "天" => TimeSpan.FromDays(amount),
                _ => TimeSpan.Zero
            };

            Log.Information("[BanParser] 封禁时长: {Amount} {Unit}", amount, unit);
            return new BanEntry
            {
                UserId = userId, ServerId = serverId, RoleName = roleName,
                Reason = message, BanTime = now,
                UnbanTime = now + duration, IsPermanent = false
            };
        }

        Log.Warning("[BanParser] 无法解析封禁时长，标记为未知封禁: {Message}", message);
        return new BanEntry
        {
            UserId = userId, ServerId = serverId, RoleName = roleName,
            Reason = message, BanTime = now,
            UnbanTime = null, IsPermanent = true
        };
    }
}
