using System.Net.Http.Json;
using System.Text.Json;
using EastSide.Core.Api.Entities;
using EastSide.Core.Api.Entities.Auth;
using EastSide.Core.Api.Entities.System;
using Serilog;
using System;

namespace EastSide.Core.Api;

public class OxygenApi : IDisposable
{
    private static readonly Lazy<OxygenApi> _instance = new(() => new OxygenApi());
    public static OxygenApi Instance => _instance.Value;
    public static Func<HttpMessageHandler>? HandlerFactory { get; set; }

    public string BaseUrl { get; }
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _disposed;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OxygenApi()
    {
        BaseUrl = "https://api.fandmc.cn";

        HttpMessageHandler handler;
        if (HandlerFactory != null)
        {
            handler = HandlerFactory();
        }
        else
        {
            handler = new HttpClientHandler();
        }

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl, UriKind.Absolute),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<ApiResponse> SendRegisterMailAsync(string email, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/register_mail", new { email }, ct);

    public async Task<ApiResponse> VerifyCodeAsync(string email, string code, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/verify_code", new { email, code }, ct);

    public async Task<LoginResponse> RegisterAsync(string email, string username, string password, CancellationToken ct = default)
        => await PostAsync<LoginResponse>("/auth/register_next", new { email, username, password }, ct);

    public async Task<LoginResponse> LoginAsync(string usernameOrEmail, string password, CancellationToken ct = default)
        => await PostAsync<LoginResponse>("/auth/login", new { username = usernameOrEmail, password }, ct);

    public async Task<TokenAuthResponse> TokenAuthAsync(string token, CancellationToken ct = default)
        => await PostAsync<TokenAuthResponse>("/auth/token_auth", new { token }, ct);

    public async Task<UserInfoResponse> GetUserInfoAsync(string token, CancellationToken ct = default)
        => await PostAsync<UserInfoResponse>("/auth/me", new { token }, ct);

    public async Task<DurationResponse> GetDurationAsync(string token, CancellationToken ct = default)
        => await PostAsync<DurationResponse>("/auth/duration", new { token }, ct);

    public async Task<ApiResponse> UpdateAvatarAsync(string token, string avatar, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/update_avatar", new { token, avatar }, ct);

    public async Task<ApiResponse> ChangePasswordAsync(string token, string oldPassword, string newPassword, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/change_password", new { token, oldPassword, newPassword }, ct);

    public async Task<ApiResponse> SendChangeEmailCodeAsync(string token, string email, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/send_change_email_code", new { token, email }, ct);

    public async Task<ApiResponse> ChangeEmailAsync(string token, string newEmail, string code, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/change_email", new { token, newEmail, code }, ct);

    public async Task<ApiResponse> SendResetPasswordCodeAsync(string email, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/send_reset_password_code", new { email }, ct);

    public async Task<ApiResponse> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken ct = default)
        => await PostAsync<ApiResponse>("/auth/reset_password", new { email, code, newPassword }, ct);

    public async Task<CardKeyActivateResponse> ActivateCardKeyAsync(string token, string cardKey, CancellationToken ct = default)
        => await PostAsync<CardKeyActivateResponse>("/cardkey/activate", new { token, cardKey }, ct);

    public async Task<GenerateCardKeyResponse> GenerateCardKeyAsync(string key, string duration, int count = 1, string? remarks = null, CancellationToken ct = default)
        => await PostAsync<GenerateCardKeyResponse>("/cardkey/generate", new { key, duration, count, remarks }, ct);

    public async Task<UserUrlResponse> GenerateUserUrlAsync(string accountToken, CancellationToken ct = default)
        => await PostAsync<UserUrlResponse>("/api/generate_user_url", new { accountToken }, ct);

    public async Task<CrcSaltResponse> GetCrcSaltAsync(string token, CancellationToken ct = default)
        => await PostAsync<CrcSaltResponse>("/api/get/crcsalt", new { token }, ct);

    public async Task<Stream?> DownloadFileAsync(string filename, CancellationToken ct = default)
    {
        try
        {
            var effectiveCt = ct == default ? CancellationToken.None : ct;
            var response = await _http.GetAsync($"/download/{filename}", HttpCompletionOption.ResponseHeadersRead, effectiveCt);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(effectiveCt);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "下载文件失败: {Filename}", filename);
            return null;
        }
    }

    public async Task<HttpResponseMessage?> DownloadFileWithRangeAsync(string filename, long? rangeStart = null, long? rangeEnd = null, CancellationToken ct = default)
    {
        try
        {
            var effectiveCt = ct == default ? CancellationToken.None : ct;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/download/{filename}");
            if (rangeStart.HasValue)
            {
                if (rangeEnd.HasValue)
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(rangeStart.Value, rangeEnd.Value);
                else
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(rangeStart.Value, null);
            }

            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, effectiveCt);
            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "下载文件失败: {Filename}", filename);
            return null;
        }
    }

    public async Task<List<PluginInfo>> GetPluginListAsync(CancellationToken ct = default)
    {
        try
        {
            var effectiveCt = ct == default ? CancellationToken.None : ct;
            using var response = await _http.GetAsync("/get/pluginlist", effectiveCt);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(effectiveCt);
            try
            {
                var list = JsonSerializer.Deserialize<List<PluginInfo>>(json, JsonOptions);
                return list ?? new List<PluginInfo>();
            }
            catch
            {
                var obj = JsonSerializer.Deserialize<PluginListResponse>(json, JsonOptions);
                var plugins = obj?.Plugins ?? new List<PluginInfo>();

                if (plugins.Count == 0)
                {
                    try
                    {
                        var dynamicObj = JsonSerializer.Deserialize<PluginListWithItems>(json, JsonOptions);
                        plugins = dynamicObj?.Items ?? new List<PluginInfo>();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("解析items字段失败: {Message}", ex.Message);
                    }
                }

                Log.Information("最终插件数量: {Count}", plugins.Count);
                return plugins;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取插件列表失败");
            return new List<PluginInfo>();
        }
    }

    public async Task<AnnouncementResponse> GetAnnouncementAsync(CancellationToken ct = default)
    {
        try
        {
            var effectiveCt = ct == default ? CancellationToken.None : ct;
            using var response = await _http.GetAsync("/get/announcement", effectiveCt);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(effectiveCt);
            return JsonSerializer.Deserialize<AnnouncementResponse>(json, JsonOptions) ?? new AnnouncementResponse { Success = false, Message = "解析失败" };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取公告失败");
            return new AnnouncementResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<VersionResponse> GetLatestVersionAsync(CancellationToken ct = default)
    {
        try
        {
            var effectiveCt = ct == default ? CancellationToken.None : ct;
            using var response = await _http.GetAsync("/get/lastversion", effectiveCt);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(effectiveCt);
            return JsonSerializer.Deserialize<VersionResponse>(json, JsonOptions) ?? new VersionResponse { Success = false, Message = "解析失败" };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取最新版本失败");
            return new VersionResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<ServerStatusResponse> GetServerStatusAsync(string key, CancellationToken ct = default)
        => await PostAsync<ServerStatusResponse>("/get/status", new { key }, ct);

    public async Task<string?> RecognizeCaptchaAsync(string base64Image, CancellationToken ct = default)
    {
        try
        {
            var response = await PostAsync<CaptchaResponse>("/v9/captcha", new { base64 = base64Image }, ct);
            return response.Result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "验证码识别失败");
            return null;
        }
    }

    public async Task<string?> RecognizeCaptchaSyncAsync(string base64Image, CancellationToken ct = default)
    {
        try
        {
            var response = await PostAsync<CaptchaResponse>("/v9/captcha/sync", new { base64 = base64Image }, ct);
            return response.Result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "验证码识别失败");
            return null;
        }
    }

    public async Task<SmResultResponse> Send4399SmAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var response = await PostAsync<SmResultResponse>("/4399/sm", new { username, password }, ct);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "4399自动实名认证请求失败");
            return new SmResultResponse { Success = false, Message = ex.Message, Code = 0 };
        }
    }

    public async Task<DeriveUuidResponse> DeriveUuidAsync(string profile, string user, string name, CancellationToken ct = default)
    {
        var effectiveCt = ct == default ? CancellationToken.None : ct;
        try
        {
            using var resp = await _http.PostAsJsonAsync("/auth/derive-uuid", new { profile, user, name }, JsonOptions, effectiveCt);
            var json = await resp.Content.ReadAsStringAsync(effectiveCt);

            var result = JsonSerializer.Deserialize<DeriveUuidResponse>(json, JsonOptions);
            if (result != null) return result;

            return new DeriveUuidResponse { Success = false, Message = "响应解析失败" };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UUID派生请求失败");
            return new DeriveUuidResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<Md5MappingResponse> GetMd5MappingAsync(string token, string version, CancellationToken ct = default)
        => await PostAsync<Md5MappingResponse>("/api/get/md5mapping", new { token, version }, ct);

    private async Task<T> PostAsync<T>(string path, object body, CancellationToken ct) where T : class, new()
    {
        var effectiveCt = ct == default ? CancellationToken.None : ct;
        await _gate.WaitAsync(effectiveCt);
        try
        {
            using var resp = await _http.PostAsJsonAsync(path, body, JsonOptions, effectiveCt);
            var json = await resp.Content.ReadAsStringAsync(effectiveCt);

            try
            {
                var result = JsonSerializer.Deserialize<T>(json, JsonOptions);
                if (result != null)
                {
                    return result;
                }
            }
            catch (JsonException)
            {
            }

            if (!resp.IsSuccessStatusCode)
            {
                Log.Error("网络请求失败: {Path}, StatusCode: {StatusCode}, Response: {Response}",
                    path, resp.StatusCode, json);
                var error = new T();
                if (error is ApiResponse apiResp)
                {
                    apiResp.Success = false;
                    apiResp.Message = $"请求失败: {path} ({resp.StatusCode})";
                }
                return error;
            }

            Log.Error("解析响应失败: {Path}, JSON: {Json}", path, json);
            var parseError = new T();
            if (parseError is ApiResponse apiResp2)
            {
                apiResp2.Success = false;
                apiResp2.Message = $"响应解析失败: {path}";
            }
            return parseError;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("请求超时: {Path}", path);
            var error = new T();
            if (error is ApiResponse apiResp)
            {
                apiResp.Success = false;
                apiResp.Message = $"请求超时: {path}";
            }
            return error;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "未知错误: {Path}", path);
            var error = new T();
            if (error is ApiResponse apiResp)
            {
                apiResp.Success = false;
                apiResp.Message = "请求失败: " + ex.Message;
            }
            return error;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _http.Dispose();
        _gate.Dispose();
        GC.SuppressFinalize(this);
    }
}
