using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;

#region 响应实体

public class RentalServerEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("icon_index")] public uint IconIndex { get; set; }
	[JsonPropertyName("visibility")] public int Visibility { get; set; }
	[JsonPropertyName("has_pwd")] public int HasPwd { get; set; }
	[JsonPropertyName("server_type")] public string ServerType { get; set; } = string.Empty;
	[JsonPropertyName("status")] public int Status { get; set; }
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("mc_version")] public string McVersion { get; set; } = string.Empty;
	[JsonPropertyName("owner_id")] public string OwnerId { get; set; } = string.Empty;
	[JsonPropertyName("player_count")] public uint PlayerCount { get; set; }
	[JsonPropertyName("image_url")] public string ImageUrl { get; set; } = string.Empty;
}

public class RentalServerDetailEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("owner_id")] public string OwnerId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("brief_summary")] public string BriefSummary { get; set; } = string.Empty;
	[JsonPropertyName("icon_index")] public uint IconIndex { get; set; }
	[JsonPropertyName("begin_time")] public ulong BeginTime { get; set; }
	[JsonPropertyName("mc_version")] public string McVersion { get; set; } = string.Empty;
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("player_count")] public uint PlayerCount { get; set; }
	[JsonPropertyName("image_url")] public string ImageUrl { get; set; } = string.Empty;
	[JsonPropertyName("status")] public int Status { get; set; }
	[JsonPropertyName("visibility")] public int Visibility { get; set; }
	[JsonPropertyName("has_pwd")] public int HasPwd { get; set; }
	[JsonPropertyName("server_type")] public string ServerType { get; set; } = string.Empty;
	[JsonPropertyName("active_components")] public List<string> ActiveComponents { get; set; } = new();
	[JsonPropertyName("update_active_components")] public List<string> UpdateActiveComponents { get; set; } = new();
}

public class RentalServerControlEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("status")] public int Status { get; set; }
}

public class RentalServerPlayerEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("server_id")] public string ServerId { get; set; } = string.Empty;
	[JsonPropertyName("user_id")] public string UserId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("create_ts")] public ulong CreateTs { get; set; }
	[JsonPropertyName("is_online")] public bool IsOnline { get; set; }
	[JsonPropertyName("status")] public int PlayerStatus { get; set; }
}

public class RentalServerPlayerLoginEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("player_count")] public uint PlayerCount { get; set; }
}

public class RentalServerWorldEnterEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("mcserver_host")] public string McserverHost { get; set; } = string.Empty;
	[JsonPropertyName("mcserver_port")] public ushort McserverPort { get; set; }
	[JsonPropertyName("state")] public int State { get; set; }
	[JsonPropertyName("cmcc_mcserver_host")] public string CmccMcserverHost { get; set; } = string.Empty;
	[JsonPropertyName("cmcc_mcserver_port")] public int CmccMcserverPort { get; set; }
	[JsonPropertyName("ctcc_mcserver_host")] public string CtccMcserverHost { get; set; } = string.Empty;
	[JsonPropertyName("ctcc_mcserver_port")] public int CtccMcserverPort { get; set; }
	[JsonPropertyName("cucc_mcserver_host")] public string CuccMcserverHost { get; set; } = string.Empty;
	[JsonPropertyName("cucc_mcserver_port")] public int CuccMcserverPort { get; set; }
	[JsonPropertyName("isp_enable")] public bool IspEnable { get; set; }
}

public class RentalServerPlayInfoEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("player_count")] public uint PlayerCount { get; set; }
}

public class RentalServerMealEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("brief_summary")] public string BriefSummary { get; set; } = string.Empty;
	[JsonPropertyName("price")] public uint Price { get; set; }
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("valid_time")] public ulong ValidTime { get; set; }
	[JsonPropertyName("status")] public int Status { get; set; }
}

public class RentalServerMealChangeEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("server_id")] public string ServerId { get; set; } = string.Empty;
	[JsonPropertyName("meal_id")] public string MealId { get; set; } = string.Empty;
	[JsonPropertyName("capacity")] public ulong Capacity { get; set; }
	[JsonPropertyName("price")] public ulong Price { get; set; }
	[JsonPropertyName("buy_type")] public int BuyType { get; set; }
}

public class RentalServerKeepEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("meal_id")] public string MealId { get; set; } = string.Empty;
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("active_time")] public ulong ActiveTime { get; set; }
	[JsonPropertyName("expire_time")] public ulong ExpireTime { get; set; }
	[JsonPropertyName("buy_type")] public int BuyType { get; set; }
}

public class RentalServerBackupEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public int BackupId { get; set; }
	[JsonPropertyName("backup_ts")] public uint BackupTs { get; set; }
	[JsonPropertyName("backup_name")] public string BackupName { get; set; } = string.Empty;
	[JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
	[JsonPropertyName("active_components")] public List<string> ActiveComponents { get; set; } = new();
}

public class RentalServerDownloadEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public int BackupId { get; set; }
	[JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
	[JsonPropertyName("size")] public long Size { get; set; }
}

public class RentalServerUploadUrlEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public uint BackupId { get; set; }
	[JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
}

public class RentalServerImageUrlEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
}

public class RentalServerWorldSettingsEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("server_id")] public string ServerId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

public class RentalServerTemplateEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("mc_version")] public string McVersion { get; set; } = string.Empty;
	[JsonPropertyName("active_components")] public List<string> ActiveComponents { get; set; } = new();
}

public class RentalServerSupportedVersionsEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("mc_server_type")] public string McServerType { get; set; } = string.Empty;
	[JsonPropertyName("version_list")] public List<string> VersionList { get; set; } = new();
}

public class RentalServerUnstablePluginEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("iids")] public List<string> Iids { get; set; } = new();
}

public class MyRentalServerEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("buy_type")] public int BuyType { get; set; }
	[JsonPropertyName("owner_id")] public string OwnerId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("brief_summary")] public string BriefSummary { get; set; } = string.Empty;
	[JsonPropertyName("icon_index")] public uint IconIndex { get; set; }
	[JsonPropertyName("active_time")] public ulong ActiveTime { get; set; }
	[JsonPropertyName("expire_time")] public ulong ExpireTime { get; set; }
	[JsonPropertyName("begin_time")] public ulong BeginTime { get; set; }
	[JsonPropertyName("meal_id")] public string MealId { get; set; } = string.Empty;
	[JsonPropertyName("mc_version")] public string McVersion { get; set; } = string.Empty;
	[JsonPropertyName("player_count")] public uint PlayerCount { get; set; }
	[JsonPropertyName("image_url")] public string ImageUrl { get; set; } = string.Empty;
	[JsonPropertyName("capacity")] public uint Capacity { get; set; }
	[JsonPropertyName("status")] public int Status { get; set; }
	[JsonPropertyName("visibility")] public int Visibility { get; set; }
	[JsonPropertyName("has_pwd")] public int HasPwd { get; set; }
	[JsonPropertyName("pwd")] public string Pwd { get; set; } = string.Empty;
	[JsonPropertyName("server_type")] public string ServerType { get; set; } = string.Empty;
	[JsonPropertyName("percent")] public int Percent { get; set; }
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("active_slot_id")] public string ActiveSlotId { get; set; } = string.Empty;
	[JsonPropertyName("active_components")] public List<string> ActiveComponents { get; set; } = new();
	[JsonPropertyName("update_active_components")] public List<string> UpdateActiveComponents { get; set; } = new();
}

public class MyRentalServerInfoEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("gamewrapper_host")] public string GamewrapperHost { get; set; } = string.Empty;
	[JsonPropertyName("gamewrapper_port")] public long GamewrapperPort { get; set; }
	[JsonPropertyName("gamewrapper_protocol")] public string GamewrapperProtocol { get; set; } = string.Empty;
	[JsonPropertyName("save_size")] public ulong SaveSize { get; set; }
	[JsonPropertyName("mc_size")] public ulong McSize { get; set; }
	[JsonPropertyName("allow_size")] public ulong AllowSize { get; set; }
}

public class MyRentalServerJobEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("job_type")] public string JobType { get; set; } = string.Empty;
	[JsonPropertyName("step")] public int Step { get; set; }
}

public class RentalServerMCAdminTokenEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("session")] public string Session { get; set; } = string.Empty;
}

public class RentalServerBackupRestoreEntity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public uint BackupId { get; set; }
}

#endregion

#region 请求实体

public class RentalServerControlRequest
{
	[JsonPropertyName("status")] public int Status { get; set; }
	[JsonPropertyName("async")] public int Async { get; set; } = 1;
	[JsonPropertyName("server_id")] public string ServerId { get; set; } = string.Empty;
}

public class SearchByServerRequest
{
	[JsonPropertyName("server_id")] public string ServerId { get; set; } = string.Empty;
	[JsonPropertyName("status")] public int Status { get; set; }
	[JsonPropertyName("offset")] public int Offset { get; set; }
	[JsonPropertyName("length")] public int Length { get; set; } = 50;
	[JsonPropertyName("is_online")] public bool IsOnline { get; set; }
}

public class RentalServerDownloadRequest
{
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
	[JsonPropertyName("backup_id")] public int BackupId { get; set; }
}

public class WorldIdRequest
{
	[JsonPropertyName("world_id")] public string WorldId { get; set; } = string.Empty;
}

#endregion
