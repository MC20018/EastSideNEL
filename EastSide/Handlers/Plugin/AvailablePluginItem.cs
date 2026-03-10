using System.ComponentModel;
using System.Runtime.CompilerServices;
using EastSide.Core.Cache;
using Serilog;

namespace EastSide.Handlers.Plugin
{
    public class AvailablePluginItem : INotifyPropertyChanged
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string Depends { get; set; } = string.Empty;

        private byte[]? _logoData;
        public byte[]? LogoData
        {
            get => _logoData;
            set
            {
                if (_logoData != value)
                {
                    _logoData = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isInstalled;
        public bool IsInstalled
        {
            get => _isInstalled;
            set
            {
                if (_isInstalled != value)
                {
                    _isInstalled = value;
                    OnPropertyChanged();
                }
            }
        }

        public async System.Threading.Tasks.Task LoadLogoAsync()
        {
            if (string.IsNullOrEmpty(LogoUrl))
                return;

            try
            {
                var data = await ImageCacheManager.GetOrDownloadAsync(LogoUrl);
                if (data == null || data.Length == 0)
                    return;

                LogoData = data;
            }
            catch (System.Exception ex)
            {
                Log.Warning(ex, "加载插件图标失败: {Url}", LogoUrl);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
