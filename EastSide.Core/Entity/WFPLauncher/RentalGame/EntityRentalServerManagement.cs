using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codexus.Cipher.Entities.WPFLauncher.RentalGame;

namespace EastSide.Core.Entity.WFPLauncher.RentalGame;

public class EntityRentalServerControl
{
	[JsonPropertyName("status")]
	public EnumServerStatus Status { get; set; }
}

public class EntityRentalServerMeal
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("brief_summary")]
	public string BriefSummary { get; set; } = "";

	[JsonPropertyName("price")]
	public uint Price { get; set; }

	[JsonPropertyName("capacity")]
	public uint Capacity { get; set; }

	[JsonPropertyName("valid_time")]
	public ulong ValidTime { get; set; }

	[JsonPropertyName("status")]
	public int Status { get; set; }
}

public class EntityRentalServerKeep
{
	[JsonPropertyName("meal_id")]
	public string MealId { get; set; } = "";

	[JsonPropertyName("capacity")]
	public uint Capacity { get; set; }
	[JsonPropertyName("active_time")]
	public ulong ActiveTime { get; set; }

	[JsonPropertyName("expire_time")]
	public ulong ExpireTime { get; set; }

	[JsonPropertyName("buy_type")]
	public int BuyType { get; set; }
}

public class EntityRentalServerMealChange
{
	[JsonPropertyName("server_id")]
	public string ServerId { get; set; } = "";

	[JsonPropertyName("meal_id")]
	public string MealId { get; set; } = "";

	[JsonPropertyName("capacity")]
	public ulong Capacity { get; set; }

	[JsonPropertyName("price")]
	public ulong Price { get; set; }

	[JsonPropertyName("buy_type")]
	public int BuyType { get; set; }
}

public class EntityRentalServerBackup
{
	[JsonPropertyName("world_id")]
	public string WorldId { get; set; } = "";

	[JsonPropertyName("backup_id")]
	public int BackupId { get; set; }

	[JsonPropertyName("backup_ts")]
	public uint BackupTs { get; set; }

	[JsonPropertyName("backup_name")]
	public string BackupName { get; set; } = "";

	[JsonPropertyName("version")]
	public string Version { get; set; } = "";

	[JsonPropertyName("active_components")]
	public List<string>? ActiveComponents { get; set; }
}

public class EntityRentalServerWorldSettings
{
	[JsonPropertyName("server_id")]
	public string ServerId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";
}

public class EntityMyRentalServer
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("buy_type")]
	public int BuyType { get; set; }

	[JsonPropertyName("owner_id")]
	public string OwnerId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("brief_summary")]
	public string BriefSummary { get; set; } = "";
	[JsonPropertyName("icon_index")]
	public uint IconIndex { get; set; }

	[JsonPropertyName("active_time")]
	public ulong ActiveTime { get; set; }

	[JsonPropertyName("expire_time")]
	public ulong ExpireTime { get; set; }

	[JsonPropertyName("begin_time")]
	public ulong BeginTime { get; set; }

	[JsonPropertyName("meal_id")]
	public string MealId { get; set; } = "";

	[JsonPropertyName("mc_version")]
	public string McVersion { get; set; } = "";

	[JsonPropertyName("player_count")]
	public uint PlayerCount { get; set; }

	[JsonPropertyName("image_url")]
	public string ImageUrl { get; set; } = "";

	[JsonPropertyName("capacity")]
	public uint Capacity { get; set; }

	[JsonPropertyName("status")]
	public EnumServerStatus Status { get; set; }

	[JsonPropertyName("visibility")]
	public EnumVisibilityStatus Visibility { get; set; }

	[JsonPropertyName("has_pwd")]
	public int HasPwd { get; set; }

	[JsonPropertyName("pwd")]
	public string Pwd { get; set; } = "";

	[JsonPropertyName("server_type")]
	public EnumServerType ServerType { get; set; }

	[JsonPropertyName("percent")]
	public int Percent { get; set; }

	[JsonPropertyName("world_id")]
	public string WorldId { get; set; } = "";

	[JsonPropertyName("active_components")]
	public List<string>? ActiveComponents { get; set; }
}

public class EntityMyRentalServerInfo
{
	[JsonPropertyName("world_id")]
	public string WorldId { get; set; } = "";

	[JsonPropertyName("gamewrapper_host")]
	public string GamewrapperHost { get; set; } = "";

	[JsonPropertyName("gamewrapper_port")]
	public long GamewrapperPort { get; set; }

	[JsonPropertyName("gamewrapper_protocol")]
	public string GamewrapperProtocol { get; set; } = "";

	[JsonPropertyName("save_size")]
	public ulong SaveSize { get; set; }

	[JsonPropertyName("mc_size")]
	public ulong McSize { get; set; }

	[JsonPropertyName("allow_size")]
	public ulong AllowSize { get; set; }
}
