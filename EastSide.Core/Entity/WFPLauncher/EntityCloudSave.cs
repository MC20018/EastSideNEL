using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityCloudSave
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("version")]
	public int Version { get; set; }

	[JsonPropertyName("update_time")]
	public long UpdateTime { get; set; }

	[JsonPropertyName("size")]
	public long Size { get; set; }

	[JsonPropertyName("vip_only")]
	public bool VipOnly { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; } = "";

	[JsonPropertyName("game_type")]
	public string GameType { get; set; } = "";

	[JsonPropertyName("mc_version")]
	public string McVersion { get; set; } = "";

	[JsonPropertyName("extra_data")]
	public string ExtraData { get; set; } = "";
}

public class EntityCloudSaveUpload
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("partnum")]
	public int PartNum { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; } = "";
}

public class EntityCloudSaveDownload
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("partnum")]
	public int PartNum { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; } = "";
}
