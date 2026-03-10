using System.Net;
using System.Text;
using System.Text.Json;
using EastSide.Core.Api;
using Serilog;

namespace EastSide.Core.Network;

public class LocalHttpServer : IDisposable
{
    private static readonly Lazy<LocalHttpServer> _instance = new(() => new LocalHttpServer());
    public static LocalHttpServer Instance => _instance.Value;

    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private bool _disposed;

    private const string Prefix = "http://localhost:18744/";

    public void Start()
    {
        if (_listener != null)
        {
            Log.Warning("本地HTTP服务器已经在运行");
            return;
        }

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(Prefix);
            _listener.Start();

            _cts = new CancellationTokenSource();
            _listenerTask = Task.Run(() => ListenAsync(_cts.Token), _cts.Token);

            Log.Information("本地HTTP服务器已启动: {Prefix}", Prefix);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "启动本地HTTP服务器失败");
        }
    }

    public void Stop()
    {
        try
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listenerTask?.Wait(TimeSpan.FromSeconds(5));
            _listener?.Close();
            _listener = null;
            Log.Information("本地HTTP服务器已停止");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "停止本地HTTP服务器失败");
        }
    }

    private async Task ListenAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener != null)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequestAsync(context, ct), ct);
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "处理HTTP请求时发生错误");
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken ct)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            if (request.Url?.AbsolutePath == "/third-party/heypixel/derive-key-v3")
            {
                if (request.HttpMethod == "GET")
                {
                    await HandleDeriveKeyGetAsync(request, response, ct);
                }
                else if (request.HttpMethod == "POST")
                {
                    await HandleDeriveKeyPostAsync(request, response, ct);
                }
                else
                {
                    response.StatusCode = 405;
                    var errorBytes = Encoding.UTF8.GetBytes("Method Not Allowed");
                    response.ContentType = "text/plain; charset=utf-8";
                    response.ContentLength64 = errorBytes.Length;
                    await response.OutputStream.WriteAsync(errorBytes, ct);
                }
            }
            else
            {
                response.StatusCode = 404;
                var errorBytes = Encoding.UTF8.GetBytes("Not Found");
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentLength64 = errorBytes.Length;
                await response.OutputStream.WriteAsync(errorBytes, ct);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "处理请求失败: {Path}", request.Url?.AbsolutePath);
            response.StatusCode = 500;
            var errorBytes = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = errorBytes.Length;
            await response.OutputStream.WriteAsync(errorBytes, ct);
        }
        finally
        {
            response.Close();
        }
    }

    private async Task HandleDeriveKeyGetAsync(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
    {
        try
        {
            var query = request.QueryString;
            var profile = query["profile"];
            var user = query["user"];
            var name = query["name"];

            if (string.IsNullOrEmpty(profile) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(name))
            {
                response.StatusCode = 400;
                var errorBytes = Encoding.UTF8.GetBytes("Missing required parameters: profile, user, name");
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentLength64 = errorBytes.Length;
                await response.OutputStream.WriteAsync(errorBytes, ct);
                return;
            }

            var result = await OxygenApi.Instance.DeriveUuidAsync(profile, user, name, ct);

            response.StatusCode = 200;
            response.ContentType = "text/plain; charset=utf-8";
            var uuidText = result?.Uuid ?? string.Empty;
            var textBytes = Encoding.UTF8.GetBytes(uuidText);
            response.ContentLength64 = textBytes.Length;
            await response.OutputStream.WriteAsync(textBytes, ct);

            Log.Information("返回UUID: {Uuid} (profile={Profile}, user={User}, name={Name})",
                uuidText, profile, user, name);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "处理derive-key GET请求失败");
            response.StatusCode = 500;
            var errorBytes = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = errorBytes.Length;
            await response.OutputStream.WriteAsync(errorBytes, ct);
        }
    }

    private async Task HandleDeriveKeyPostAsync(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
    {
        try
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            var body = await reader.ReadToEndAsync(ct);
            var requestData = JsonSerializer.Deserialize<DeriveKeyRequest>(body);

            if (requestData == null || string.IsNullOrEmpty(requestData.Profile) ||
                string.IsNullOrEmpty(requestData.User) || string.IsNullOrEmpty(requestData.Name))
            {
                response.StatusCode = 400;
                var errorBytes = Encoding.UTF8.GetBytes("Invalid request parameters");
                response.ContentType = "text/plain; charset=utf-8";
                response.ContentLength64 = errorBytes.Length;
                await response.OutputStream.WriteAsync(errorBytes, ct);
                return;
            }

            var result = await OxygenApi.Instance.DeriveUuidAsync(requestData.Profile, requestData.User, requestData.Name, ct);

            response.StatusCode = 200;
            response.ContentType = "text/plain; charset=utf-8";
            var uuidText = result?.Uuid ?? string.Empty;
            var textBytes = Encoding.UTF8.GetBytes(uuidText);
            response.ContentLength64 = textBytes.Length;
            await response.OutputStream.WriteAsync(textBytes, ct);

            Log.Information("返回UUID: {Uuid} (profile={Profile}, user={User}, name={Name})",
                uuidText, requestData.Profile, requestData.User, requestData.Name);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "处理derive-key POST请求失败");
            response.StatusCode = 500;
            var errorBytes = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = errorBytes.Length;
            await response.OutputStream.WriteAsync(errorBytes, ct);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    private class DeriveKeyRequest
    {
        [System.Text.Json.Serialization.JsonPropertyName("profile")]
        public string Profile { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("user")]
        public string User { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
