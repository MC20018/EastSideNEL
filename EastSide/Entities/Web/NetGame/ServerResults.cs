using System.Collections.Generic;
using System.ComponentModel;

namespace EastSide.Entities.Web.NetGame;

public class ServerItem : INotifyPropertyChanged
{
    private string _entityId = string.Empty;
    private string _name = string.Empty;
    private string _imageUrl = string.Empty;
    private string _onlineCount = string.Empty;

    public string EntityId { get => _entityId; set { _entityId = value; OnPropertyChanged(nameof(EntityId)); } }
    public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
    public string ImageUrl { get => _imageUrl; set { _imageUrl = value; OnPropertyChanged(nameof(ImageUrl)); } }
    public string OnlineCount { get => _onlineCount; set { _onlineCount = value; OnPropertyChanged(nameof(OnlineCount)); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class ListServersResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ServerItem> Items { get; set; } = new();
    public bool HasMore { get; set; }
}

public class ServerDetailResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Images { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

public class RoleItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ServerRolesResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string ServerId { get; set; } = string.Empty;
    public List<RoleItem> Items { get; set; } = new();
}

public class JoinGameResult
{
    public bool Success { get; set; }
    public bool NotLogin { get; set; }
    public string? Message { get; set; }
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
}

public class ServerResourceMod
{
    public string Name { get; set; } = string.Empty;
    public string Md5 { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class ServerResourceResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string GameVersion { get; set; } = string.Empty;
    public string BootstrapMd5 { get; set; } = string.Empty;
    public string DatFileMd5 { get; set; } = string.Empty;
    public string ServerIp { get; set; } = string.Empty;
    public int ServerPort { get; set; }
    public List<ServerResourceMod> Mods { get; set; } = new();
}
