using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EastSide.Core.Api;
using EastSide.Core.Api.Entities.System;
using Serilog;

namespace EastSide.Handlers.Plugin
{
    public class ListAvailablePlugins
    {
        public async Task<List<AvailablePluginItem>> Execute()
        {
            try
            {
                var plugins = await OxygenApi.Instance.GetPluginListAsync();
                return plugins.Select(p => new AvailablePluginItem
                {
                    Id = (p.Id ?? string.Empty).ToUpperInvariant(),
                    Name = p.Name ?? string.Empty,
                    Version = p.Version ?? string.Empty,
                    LogoUrl = (p.LogoUrl ?? string.Empty).Replace("`", string.Empty).Trim(),
                    ShortDescription = p.ShortDescription ?? string.Empty,
                    Publisher = p.Publisher ?? string.Empty,
                    DownloadUrl = (p.DownloadUrl ?? string.Empty).Replace("`", string.Empty).Trim(),
                    Depends = (p.Depends ?? string.Empty).ToUpperInvariant()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "获取插件列表失败");
                return new List<AvailablePluginItem>();
            }
        }
    }
}
