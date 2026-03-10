namespace EastSide.Entities.Web.Lobby;

public class LobbyRoomItem
{
    public uint Hid { get; set; }
    public uint Rid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tips { get; set; } = string.Empty;
    public uint Cap { get; set; }
    public uint Free { get; set; }
    public uint Type { get; set; }
    public string Srv { get; set; } = string.Empty;
    public List<uint> TagIds { get; set; } = new();
}

public class ListLobbyRoomsResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<LobbyRoomItem> Items { get; set; } = new();
    public bool HasMore { get; set; }
}

public class LobbyActionResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class LobbyRoomDetailResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public LobbyRoomItem? Room { get; set; }
}

public class LobbyRoomMembersResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<uint> UidList { get; set; } = new();
}

public class LobbyRoleItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class LobbyRolesResult
{
    public bool NotLogin { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<LobbyRoleItem> Items { get; set; } = new();
}

public class EntityJoinLobbyRoom
{
    public string AccountId { get; set; } = string.Empty;
    public uint Hid { get; set; }
    public uint Rid { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string Srv { get; set; } = string.Empty;
    public string McVersion { get; set; } = string.Empty;
}
