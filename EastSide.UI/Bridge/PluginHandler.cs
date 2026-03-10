using System;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Handlers.Plugin;
using Serilog;

namespace EastSide.UI.Bridge;

public static class PluginHandler
{
    public static async Task<BridgeResponse> ListAvailable(BridgeRequest req)
    {
        try
        {
            var items = await new ListAvailablePlugins().Execute();
            var installedIds = new ListInstalledPlugins()
                .Execute()
                .Select(p => p.Id.ToUpperInvariant())
                .ToHashSet();

            var plugins = items.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                version = p.Version,
                logoUrl = p.LogoUrl,
                shortDescription = p.ShortDescription,
                publisher = p.Publisher,
                isInstalled = installedIds.Contains(p.Id)
            }).ToList();

            return BridgeResponse.Ok(req, new { plugins });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取插件列表失败");
            return BridgeResponse.Fail(req, "获取插件列表失败");
        }
    }

    public static Task<BridgeResponse> ListInstalled(BridgeRequest req)
    {
        try
        {
            var items = new ListInstalledPlugins().Execute();
            var plugins = items.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                version = p.Version,
                author = p.Author,
                status = p.Status
            }).ToList();

            return Task.FromResult(BridgeResponse.Ok(req, new { plugins }));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取已安装插件失败");
            return Task.FromResult(BridgeResponse.Fail(req, "获取已安装插件失败"));
        }
    }

    public static async Task<BridgeResponse> Install(BridgeRequest req)
    {
        if (req.Data == null) return BridgeResponse.Fail(req, "缺少参数");
        var pluginId = req.Data.Value.GetProperty("pluginId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(pluginId))
            return BridgeResponse.Fail(req, "缺少 pluginId");

        try
        {
            var available = await new ListAvailablePlugins().Execute();
            var item = available.FirstOrDefault(p =>
                string.Equals(p.Id, pluginId, StringComparison.OrdinalIgnoreCase));

            if (item == null)
                return BridgeResponse.Fail(req, "插件不存在");

            await new InstallPlugin().Execute(item);
            return BridgeResponse.Ok(req, new { message = $"插件 {item.Name} 安装成功" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "安装插件失败");
            return BridgeResponse.Fail(req, "安装失败: " + ex.Message);
        }
    }

    public static Task<BridgeResponse> Uninstall(BridgeRequest req)
    {
        if (req.Data == null) return Task.FromResult(BridgeResponse.Fail(req, "缺少参数"));
        var pluginId = req.Data.Value.GetProperty("pluginId").GetString() ?? "";
        if (string.IsNullOrWhiteSpace(pluginId))
            return Task.FromResult(BridgeResponse.Fail(req, "缺少 pluginId"));

        try
        {
            new UninstallPlugin().Execute(pluginId);
            return Task.FromResult(BridgeResponse.Ok(req, new { message = "卸载成功" }));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "卸载插件失败");
            return Task.FromResult(BridgeResponse.Fail(req, "卸载失败: " + ex.Message));
        }
    }
}
