using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;

#region 响应实体

public class LobbyGameRoomEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("room_name")] public string RoomName { get; set; } = string.Empty;
	[JsonPropertyName("password")] public bool Password { get; set; }
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("max_count")] public uint MaxCount { get; set; }
	[JsonPropertyName("allow_save")] public bool AllowSave { get; set; }
	[JsonPropertyName("visibility")] public int Visibility { get; set; }
	[JsonPropertyName("owner_id")] public string OwnerId { get; set; } = string.Empty;
	[JsonPropertyName("save_id")] public string SaveId { get; set; } = string.Empty;
	[JsonPropertyName("save_size")] public ulong SaveSize { get; set; }
	[JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
	[JsonPropertyName("game_status")] public int GameStatus { get; set; }
	[JsonPropertyName("fids")] public List<string> Fids { get; set; } = new();
	[JsonPropertyName("cur_num")] public uint CurNum { get; set; }
	[JsonPropertyName("has_like")] public bool HasLike { get; set; }
}

public class LobbyGameAddressEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("server_host")] public string ServerHost { get; set; } = string.Empty;
	[JsonPropertyName("server_port")] public int ServerPort { get; set; }
	[JsonPropertyName("cmcc_server_host")] public string CmccServerHost { get; set; } = string.Empty;
	[JsonPropertyName("cmcc_server_port")] public int CmccServerPort { get; set; }
	[JsonPropertyName("ctcc_server_host")] public string CtccServerHost { get; set; } = string.Empty;
	[JsonPropertyName("ctcc_server_port")] public int CtccServerPort { get; set; }
	[JsonPropertyName("cucc_server_host")] public string CuccServerHost { get; set; } = string.Empty;
	[JsonPropertyName("cucc_server_port")] public int CuccServerPort { get; set; }
	[JsonPropertyName("isp_enable")] public bool IspEnable { get; set; }
}

public class LobbyRoomMemberInfoEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("member_id")] public uint MemberId { get; set; }
	[JsonPropertyName("ident")] public int Ident { get; set; }
}

public class OnlineLobbyServerStatusEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("maintenance")] public string Maintenance { get; set; } = string.Empty;
	[JsonPropertyName("mc_version")] public string McVersion { get; set; } = string.Empty;
	[JsonPropertyName("client_mc_version")] public string ClientMcVersion { get; set; } = string.Empty;
	[JsonPropertyName("client_forbidden_version")] public string ClientForbiddenVersion { get; set; } = string.Empty;
}

public class LobbyGameSaveBackupEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public string BackupId { get; set; } = string.Empty;
	[JsonPropertyName("save_id")] public string SaveId { get; set; } = string.Empty;
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("timestamp")] public ulong Timestamp { get; set; }
	[JsonPropertyName("expire_time")] public ulong ExpireTime { get; set; }
	[JsonPropertyName("size")] public ulong Size { get; set; }
	[JsonPropertyName("debackup")] public int Debackup { get; set; }
	[JsonPropertyName("inbackup")] public int Inbackup { get; set; }
}

public class LobbyCollectionEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("update_time")] public ulong UpdateTime { get; set; }
	[JsonPropertyName("user_id")] public string UserId { get; set; } = string.Empty;
}

#endregion

#region 请求实体

public class CreateLobbyGameRoomRequest
{
	[JsonPropertyName("room_name")] public string RoomName { get; set; } = string.Empty;
	[JsonPropertyName("max_count")] public uint MaxCount { get; set; }
	[JsonPropertyName("visibility")] public uint Visibility { get; set; }
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("save_id")] public string SaveId { get; set; } = string.Empty;
	[JsonPropertyName("password")] public string Password { get; set; } = string.Empty;
}

public class LobbyRoomEnterRequest
{
	[JsonPropertyName("room_id")] public string RoomId { get; set; } = string.Empty;
	[JsonPropertyName("password")] public string Password { get; set; } = string.Empty;
	[JsonPropertyName("check_visibilily")] public bool CheckVisibility { get; set; }
}

public class LobbyGameRoomKickMemberRequest
{
	[JsonPropertyName("room_id")] public string RoomId { get; set; } = string.Empty;
	[JsonPropertyName("user_id")] public uint UserId { get; set; }
}

public class LaunchRoomRequest
{
	[JsonPropertyName("room_id")] public string RoomId { get; set; } = string.Empty;
}

public class ResetRoomSettingRequest
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("visibility")] public int Visibility { get; set; }
	[JsonPropertyName("allow_save")] public bool AllowSave { get; set; }
	[JsonPropertyName("owner_id")] public uint OwnerId { get; set; }
	[JsonPropertyName("room_id")] public string RoomId { get; set; } = string.Empty;
}

public class SearchLobbyRoomsByItemIdRequest
{
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("offset")] public int Offset { get; set; }
	[JsonPropertyName("length")] public int Length { get; set; }
	[JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
	[JsonPropertyName("with_friend")] public bool WithFriend { get; set; } = true;
}

public class SearchByRoomNameRequest
{
	[JsonPropertyName("room_name")] public string RoomName { get; set; } = string.Empty;
	[JsonPropertyName("res_id")] public string ResId { get; set; } = string.Empty;
	[JsonPropertyName("offset")] public int Offset { get; set; }
	[JsonPropertyName("length")] public int Length { get; set; }
	[JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
}

#endregion
