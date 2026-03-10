using System.Collections.Generic;
using Codexus.Development.SDK.Manager;

namespace EastSide.Handlers.Plugin
{
    public class ListInstalledPlugins
    {
        public List<PluginViewModel> Execute()
        {
            var list = new List<PluginViewModel>();
            foreach (var plugin in PluginManager.Instance.Plugins.Values)
            {
                list.Add(new PluginViewModel
                {
                    Id = plugin.Id,
                    Name = plugin.Name,
                    Description = plugin.Description,
                    Version = plugin.Version,
                    Author = plugin.Author,
                    Status = plugin.Status,
                    NeedUpdate = false
                });
            }
            return list;
        }
    }
}
