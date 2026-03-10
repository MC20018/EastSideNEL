using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EastSide.Core.Api;

namespace EastSide.Manager;

public sealed class AuthManager
{
    public static AuthManager Instance { get; } = new AuthManager();
    
    public const bool SkipLogin = false;

    public const string FallbackSalt = "";

    static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public string Token { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public long UserId { get; private set; }
    public string? Avatar { get; private set; }
    public string? Rank { get; private set; }
    public bool IsBanned { get; private set; }
    public bool IsAdmin { get; private set; }
    public string CachedSalt { get; private set; } = FallbackSalt;
    public string CachedGameVersion { get; private set; } = string.Empty;
    public DateTime? TrialExpiryDate { get; private set; }
    public DateTime? MembershipExpiryDate { get; private set; }
    public bool IsLoggedIn => SkipLogin || !string.IsNullOrWhiteSpace(Token);

    public async Task<string> GetCrcSaltAsyncIfNeeded(CancellationToken ct = default)
    {
        if (SkipLogin) return FallbackSalt;
        if (string.IsNullOrEmpty(CachedSalt))
        {
            await GetCrcSaltAsync(ct);
        }
        return CachedSalt;
    }

    public string GetAuthFilePath()
    {
        var baseDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
        return Path.Combine(baseDir, "auth.dat");
    }

    public void LoadFromDisk()
    {
        try
        {
            var path = GetAuthFilePath();
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json)) return;
            var data = JsonSerializer.Deserialize<AuthData>(json, JsonOptions);
            if (data == null) return;
            if (string.IsNullOrWhiteSpace(data.Token)) return;
            Token = data.Token.Trim();
            Username = data.Username?.Trim() ?? string.Empty;
            Email = data.Email?.Trim() ?? string.Empty;
            Avatar = data.Avatar;
            Rank = data.Rank;
            UserId = data.UserId ?? 0;
            IsAdmin = data.IsAdmin ?? false;
            if (!string.IsNullOrEmpty(data.TrialExpiryDate) && DateTime.TryParse(data.TrialExpiryDate, out var trialDate))
            {
                TrialExpiryDate = trialDate;
            }
            if (!string.IsNullOrEmpty(data.MembershipExpiryDate) && DateTime.TryParse(data.MembershipExpiryDate, out var memberDate))
            {
                MembershipExpiryDate = memberDate;
            }
            Log.Information("从磁盘加载用户数据: UserId={UserId}, Username={Username}, HasAvatar={HasAvatar}", 
                UserId, Username, !string.IsNullOrEmpty(Avatar));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "读取 auth.dat 失败");
        }
    }

    public void SaveToDisk()
    {
        try
        {
            var path = GetAuthFilePath();
            var data = new AuthData 
            { 
                Token = Token, 
                Username = Username,
                Email = Email,
                Avatar = Avatar,
                Rank = Rank,
                UserId = UserId,
                IsAdmin = IsAdmin,
                TrialExpiryDate = TrialExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                MembershipExpiryDate = MembershipExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "保存 auth.dat 失败");
        }
    }

    public void Clear()
    {
        Token = string.Empty;
        Username = string.Empty;
        Email = string.Empty;
        UserId = 0;
        Avatar = null;
        Rank = null;
        IsBanned = false;
        IsAdmin = false;
        TrialExpiryDate = null;
        MembershipExpiryDate = null;
        try
        {
            var path = GetAuthFilePath();
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "删除 auth.dat 失败");
        }
    }

    public async Task<ApiResult> SendRegisterMailAsync(string email, CancellationToken ct = default)
    {
        var resp = await OxygenApi.Instance.SendRegisterMailAsync(email, ct);
        return new ApiResult(resp.Success, resp.Message ?? (resp.Success ? "成功" : "失败"));
    }

    public async Task<ApiResult> VerifyCodeAsync(string email, string code, CancellationToken ct = default)
    {
        var resp = await OxygenApi.Instance.VerifyCodeAsync(email, code, ct);
        return new ApiResult(resp.Success, resp.Message ?? (resp.Success ? "成功" : "失败"));
    }

    public async Task<ApiResult> RegisterNextAsync(string email, string username, string password, CancellationToken ct = default)
    {
        var resp = await OxygenApi.Instance.RegisterAsync(email, username, password, ct);
        if (!resp.Success) return new ApiResult(false, resp.Message ?? "注册失败");
        if (resp.Token != null)
        {
            Token = resp.Token;
            Username = username;
            Email = email;
            SaveToDisk();
            _ = FetchUserInfoAsync(ct);
        }
        return new ApiResult(true, resp.Message ?? "成功", resp.Token);
    }

    public async Task<ApiResult> LoginAsync(string usernameOrEmail, string password, CancellationToken ct = default)
    {
        var resp = await OxygenApi.Instance.LoginAsync(usernameOrEmail, password, ct);
        if (!resp.Success) return new ApiResult(false, resp.Message ?? "登录失败");
        if (resp.Token == null) return new ApiResult(false, "登录失败：未返回 token");
        Token = resp.Token;
        SaveToDisk();
        _ = FetchUserInfoAsync(ct);
        return new ApiResult(true, resp.Message ?? "成功", resp.Token);
    }

    public async Task<TokenAuthResult> TokenAuthAsync(CancellationToken ct = default)
    {
        if (!IsLoggedIn) return new TokenAuthResult(false, "未登录");

        var resp = await OxygenApi.Instance.TokenAuthAsync(Token, ct);
        if (!resp.Success)
        {
            Log.Warning("Token认证失败: {Message}", resp.Message);
            return new TokenAuthResult(false, resp.Message ?? "认证失败");
        }

        if (resp.User != null)
        {
            UserId = resp.User.Id;
            Username = resp.User.Username ?? string.Empty;
            Email = resp.User.Email ?? string.Empty;
            Rank = resp.User.Rank;
            IsAdmin = resp.User.IsAdmin;
        }
        Log.Information("Token认证成功: UserId={UserId}, Username={Username}", UserId, Username);
        return new TokenAuthResult(true, "成功");
    }

    public async Task<UserInfoResult> FetchUserInfoAsync(CancellationToken ct = default)
    {
        if (!IsLoggedIn) return new UserInfoResult(false, "未登录");

        var resp = await OxygenApi.Instance.GetUserInfoAsync(Token, ct);
        if (!resp.Success)
        {
            Log.Warning("获取用户信息失败: {Message}", resp.Message);
            return new UserInfoResult(false, resp.Message ?? "获取失败");
        }

        UserId = resp.Id ?? 0;
        Username = resp.Username ?? string.Empty;
        Email = resp.Email ?? string.Empty;
        Avatar = resp.Avatar;
        Rank = resp.Rank;
        IsBanned = resp.Banned == 1;
        IsAdmin = resp.IsAdmin == 1;
        Log.Information("用户信息已更新: UserId={UserId}, Username={Username}, HasAvatar={HasAvatar}", UserId, Username, !string.IsNullOrEmpty(Avatar));
        return new UserInfoResult(true, "成功");
    }

    public async Task<CrcSaltResult> GetCrcSaltAsync(CancellationToken ct = default)
    {
        if (!IsLoggedIn) return new CrcSaltResult(false, "未登录", null, null, null);
        
        if (!string.IsNullOrEmpty(CachedSalt))
            return new CrcSaltResult(true, "成功", CachedSalt, CachedGameVersion, UserId);

        Log.Information("[CRC] GetCrcSaltAsync: 缓存为空，从 Codexus 服务获取");
        var resp = await OxygenApi.Instance.GetCrcSaltAsync(string.Empty, ct);
        Log.Information("[CRC] GetCrcSaltAsync: 服务器返回 Success={Success}, Salt={Salt}, Msg={Msg}", 
            resp.Success, resp.Salt ?? "(null)", resp.Message ?? "(null)");
        
        if (!resp.Success)
        {
            return new CrcSaltResult(false, resp.Message ?? "获取失败", null, null, null);
        }

        CachedSalt = resp.Salt ?? string.Empty;
        CachedGameVersion = resp.GameVersion ?? string.Empty;
        return new CrcSaltResult(true, "成功", CachedSalt, CachedGameVersion, UserId);
    }

    public void ClearCrcSaltCache()
    {
        CachedSalt = string.Empty;
        CachedGameVersion = string.Empty;
    }

    public async Task<UserUrlResult> GenerateUserUrlAsync(CancellationToken ct = default)
    {
        if (!IsLoggedIn) return new UserUrlResult(false, "未登录", null);

        var resp = await OxygenApi.Instance.GenerateUserUrlAsync(Token, ct);
        if (!resp.Success)
        {
            return new UserUrlResult(false, resp.Message ?? "获取失败", null);
        }

        return new UserUrlResult(true, "成功", resp.UserUrl);
    }

    public void ActivateTrial(int days = 15)
    {
        TrialExpiryDate = DateTime.Now.AddDays(days);
        SaveToDisk();
        Log.Information("试用已激活，到期时间: {ExpiryDate}", TrialExpiryDate);
    }

    public bool IsTrialActive()
    {
        if (TrialExpiryDate == null) return false;
        return TrialExpiryDate.Value > DateTime.Now;
    }

    public bool HasFeatureAccess()
    {
        if (IsTrialActive()) return true;
        
        if (MembershipExpiryDate.HasValue && MembershipExpiryDate.Value > DateTime.Now)
            return true;
        
        return false;
    }

    public bool IsPermanentMember => MembershipExpiryDate == DateTime.MaxValue;

    public bool HasValidMembership => MembershipExpiryDate.HasValue && MembershipExpiryDate.Value > DateTime.Now;

    public int GetDaysLeft()
    {
        if (IsPermanentMember) return int.MaxValue;
        if (!MembershipExpiryDate.HasValue) return 0;
        var days = (int)Math.Ceiling((MembershipExpiryDate.Value - DateTime.Now).TotalDays);
        return Math.Max(0, days);
    }

    public string GetFormattedDaysLeft()
    {
        var days = GetDaysLeft();
        if (days == int.MaxValue) return "永久";
        if (days > 36500) return "永久";
        if (days > 365)
        {
            var years = days / 365;
            var remainingDays = days % 365;
            return remainingDays > 0 ? $"{years}年{remainingDays}天" : $"{years}年";
        }
        return $"{days}天";
    }

    public string GetFormattedExpiryDate()
    {
        if (IsPermanentMember) return "永久";
        if (!MembershipExpiryDate.HasValue) return "未设置";
        return MembershipExpiryDate.Value.ToString("yyyy/MM/dd/");
    }

    public enum MembershipStatus
    {
        None,
        Active,
        Expired,
        Permanent,
        Trial
    }

    public MembershipStatus GetMembershipStatus()
    {
        if (IsPermanentMember) return MembershipStatus.Permanent;
        
        if (HasValidMembership) return MembershipStatus.Active;
        
        if (IsTrialActive()) return MembershipStatus.Trial;
        
        if (MembershipExpiryDate.HasValue && MembershipExpiryDate.Value <= DateTime.Now)
            return MembershipStatus.Expired;
        
        return MembershipStatus.None;
    }
    
    public async Task<bool> RefreshFeatureAccessAsync()
    {
        if (string.IsNullOrWhiteSpace(Token)) return false;
        
        try
        {
            var response = await OxygenApi.Instance.GetDurationAsync(Token);
            if (!response.Success)
            {
                MembershipExpiryDate = null;
                return IsTrialActive();
            }
            
            var duration = response.Duration ?? "未设置";
            
            if (duration == "永久")
            {
                MembershipExpiryDate = DateTime.MaxValue;
                SaveToDisk();
                return true;
            }
            
            if (duration == "未设置")
            {
                MembershipExpiryDate = null;
                return IsTrialActive();
            }
            
            if (IsFarFutureDate(duration))
            {
                MembershipExpiryDate = DateTime.MaxValue;
                SaveToDisk();
                return true;
            }
            
            if (DateTime.TryParse(duration.TrimEnd('/'), out var expiryDate))
            {
                var expiryEndOfDay = expiryDate.Date.AddDays(1).AddSeconds(-1);
                MembershipExpiryDate = expiryEndOfDay;
                SaveToDisk();
                return expiryEndOfDay > DateTime.Now || IsTrialActive();
            }
            
            MembershipExpiryDate = null;
            return IsTrialActive();
        }
        catch
        {
            return HasFeatureAccess();
        }
    }

    private static bool IsFarFutureDate(string duration)
    {
        var trimmed = duration.TrimEnd('/');
        var parts = trimmed.Split('/');
        if (parts.Length >= 1 && int.TryParse(parts[0], out var year))
        {
            return year > 9999;
        }
        return false;
    }

    sealed class AuthData
    {
        public string Token { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string? Rank { get; set; }
        public long? UserId { get; set; }
        public bool? IsAdmin { get; set; }
        public string? TrialExpiryDate { get; set; }
        public string? MembershipExpiryDate { get; set; }
    }
}

public readonly record struct ApiResult(bool Success, string Message, string? Token = null);
public readonly record struct UserInfoResult(bool Success, string Message);
public readonly record struct TokenAuthResult(bool Success, string Message);
public readonly record struct CrcSaltResult(bool Success, string Message, string? Salt, string? GameVersion, long? Id);
public readonly record struct UserUrlResult(bool Success, string Message, string? UserUrl);
