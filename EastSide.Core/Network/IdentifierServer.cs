using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace EastSide.Core.Network;

public class IdentifierServer : IDisposable
{
    private static readonly Lazy<IdentifierServer> _instance = new(() => new IdentifierServer());
    public static IdentifierServer Instance => _instance.Value;

    private const string Prefix = "http://127.0.0.1:23333/";
    private const string Identifier = "SOUTHSIDE";

    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public Func<string, ChannelInfo?>? ChannelLookup { get; set; }

    public Action<string>? OnChannelMarked { get; set; }

    public void Start()
    {
        if (_listener != null) return;

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(Prefix);
            _listener.Start();
            _cts = new CancellationTokenSource();
            _ = Task.Run(() => ListenAsync(_cts.Token), _cts.Token);
            Log.Information("标识服务器已启动: {Prefix}", Prefix);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "启动标识服务器失败");
        }
    }
    private async Task ListenAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener != null)
        {
            try
            {
                var ctx = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleAsync(ctx, ct), ct);
            }
            catch (HttpListenerException) { break; }
            catch (Exception ex) { Log.Error(ex, "标识服务器请求处理异常"); }
        }
    }

    private async Task HandleAsync(HttpListenerContext ctx, CancellationToken ct)
    {
        var resp = ctx.Response;
        try
        {
            if (ctx.Request.Url?.AbsolutePath == "/auth/identifier")
            {
                var address = ctx.Request.QueryString["address"];
                object result;

                if (!string.IsNullOrEmpty(address) && ChannelLookup != null)
                {
                    var channel = ChannelLookup(address);
                    if (channel != null)
                    {
                        OnChannelMarked?.Invoke(address);
                        Log.Information("通道已标记为 SOUTHSIDE: {Address}", address);
                    }
                    result = new
                    {
                        identifier = Identifier,
                        found = channel != null,
                        channel = channel != null ? new
                        {
                            id = channel.Identifier,
                            serverName = channel.ServerName,
                            roleName = channel.RoleName,
                            serverVersion = channel.ServerVersion,
                            localAddress = channel.LocalAddress,
                            forwardAddress = channel.ForwardAddress
                        } : null
                    };
                }
                else
                {
                    result = new { identifier = Identifier, found = false, channel = (object?)null };
                }

                resp.StatusCode = 200;
                resp.ContentType = "application/json; charset=utf-8";
                var json = JsonSerializer.Serialize(result);
                var data = Encoding.UTF8.GetBytes(json);
                resp.ContentLength64 = data.Length;
                await resp.OutputStream.WriteAsync(data, ct);
            }
            else
            {
                resp.StatusCode = 404;
                var data = Encoding.UTF8.GetBytes("Not Found");
                resp.ContentType = "text/plain; charset=utf-8";
                resp.ContentLength64 = data.Length;
                await resp.OutputStream.WriteAsync(data, ct);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "标识服务器响应失败");
        }
        finally
        {
            resp.Close();
        }
    }

    public void Stop()
    {
        try
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
            _listener = null;
        }
        catch { }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class ChannelInfo
{
    public string Identifier { get; set; } = "";
    public string ServerName { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string ServerVersion { get; set; } = "";
    public string LocalAddress { get; set; } = "";
    public string ForwardAddress { get; set; } = "";
}
