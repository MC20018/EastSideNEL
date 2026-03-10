using Serilog;
using EastSide.Core.Api;

namespace EastSide.Utils;

public static class HttpUrlRewriter
{
    private static readonly Dictionary<string, string> UrlMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["https://api.codexus.today/api/PluginCipher/bjd/mapping/data"] = $"{OxygenApi.Instance.BaseUrl}/bjdmapping"
    };

    private static bool _initialized;
    private static Func<HttpRequestMessage, bool>? _originalValidator;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            System.Diagnostics.DiagnosticListener.AllListeners.Subscribe(new HttpDiagnosticObserver());
            
            Log.Information("[HttpUrlRewriter] HTTP URL 重写已启用，共 {Count} 条规则", UrlMappings.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[HttpUrlRewriter] 初始化失败");
        }
    }

    public static void AddMapping(string originalUrl, string newUrl)
    {
        UrlMappings[originalUrl] = newUrl;
    }

    public static bool TryRewriteUri(ref Uri? uri)
    {
        if (uri == null) return false;
        
        var originalUrl = uri.ToString();
        if (UrlMappings.TryGetValue(originalUrl, out var newUrl))
        {
            Log.Information("[HttpUrlRewriter] URL 重写: {Original} -> {New}", originalUrl, newUrl);
            uri = new Uri(newUrl);
            return true;
        }
        return false;
    }

    public static void Shutdown()
    {
        _initialized = false;
    }

    private class HttpDiagnosticObserver : IObserver<System.Diagnostics.DiagnosticListener>
    {
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(System.Diagnostics.DiagnosticListener listener)
        {
            if (listener.Name == "HttpHandlerDiagnosticListener")
            {
                listener.Subscribe(new HttpActivityObserver());
            }
        }
    }

    private class HttpActivityObserver : IObserver<KeyValuePair<string, object?>>
    {
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(KeyValuePair<string, object?> kvp)
        {
            if (kvp.Key == "System.Net.Http.HttpRequestOut.Start" && kvp.Value != null)
            {
                try
                {
                    var requestProperty = kvp.Value.GetType().GetProperty("Request");
                    if (requestProperty?.GetValue(kvp.Value) is HttpRequestMessage request)
                    {
                        var uri = request.RequestUri;
                        if (TryRewriteUri(ref uri))
                        {
                            request.RequestUri = uri;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, "[HttpUrlRewriter] 处理请求失败");
                }
            }
        }
    }
}
