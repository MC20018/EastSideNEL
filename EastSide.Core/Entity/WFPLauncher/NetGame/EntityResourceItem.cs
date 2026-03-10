using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher.NetGame;

public class EntityResourceItem
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("item_type")]
	public int ItemType { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("item_version")]
	public string ItemVersion { get; set; } = "";

	[JsonPropertyName("developer_name")]
	public string DeveloperName { get; set; } = "";

	[JsonPropertyName("master_type_id")]
	public string MasterTypeId { get; set; } = "";

	[JsonPropertyName("secondary_type_id")]
	public string SecondaryTypeId { get; set; } = "";

	[JsonPropertyName("brief_summary")]
	public string BriefSummary { get; set; } = "";

	[JsonPropertyName("image_url")]
	public string ImageUrl { get; set; } = "";

	[JsonPropertyName("title_image_url")]
	public string TitleImageUrl { get; set; } = "";

	[JsonPropertyName("vip_only")]
	public bool VipOnly { get; set; }

	[JsonPropertyName("has_purchased")]
	public bool HasPurchased { get; set; }

	[JsonPropertyName("price")]
	public ulong Price { get; set; }

	[JsonPropertyName("diamond")]
	public int Diamond { get; set; }

	[JsonPropertyName("discount")]
	public float Discount { get; set; }

	[JsonPropertyName("vip_discount")]
	public float VipDiscount { get; set; }
	[JsonPropertyName("likes")]
	public ulong Likes { get; set; }

	[JsonPropertyName("like_num")]
	public uint LikeNum { get; set; }

	[JsonPropertyName("download_num")]
	public uint DownloadNum { get; set; }

	[JsonPropertyName("online_count")]
	public uint OnlineCount { get; set; }

	[JsonPropertyName("publish_time")]
	public ulong PublishTime { get; set; }

	[JsonPropertyName("purchase_type")]
	public string PurchaseType { get; set; } = "";

	[JsonPropertyName("mc_version_name")]
	public string McVersionName { get; set; } = "";

	[JsonPropertyName("lobby_min_num")]
	public uint LobbyMinNum { get; set; }

	[JsonPropertyName("lobby_max_num")]
	public uint LobbyMaxNum { get; set; }
}

public class EntityComponentQuery
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("iid")]
	public object? Iid { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("brief")]
	public string Brief { get; set; } = "";

	[JsonPropertyName("info")]
	public string Info { get; set; } = "";

	[JsonPropertyName("mtypeid")]
	public int MTypeId { get; set; }

	[JsonPropertyName("stypeid")]
	public int STypeId { get; set; }

	[JsonPropertyName("include_map")]
	public int IncludeMap { get; set; }

	[JsonPropertyName("resversion")]
	public int ResVersion { get; set; }

	[JsonPropertyName("versions")]
	public List<string>? Versions { get; set; }

	[JsonPropertyName("mcversion")]
	public string McVersion { get; set; } = "";

	[JsonPropertyName("vip_only")]
	public bool VipOnly { get; set; }

	[JsonPropertyName("behaviour_uuid")]
	public string BehaviourUuid { get; set; } = "";

	[JsonPropertyName("resource_uuid")]
	public string ResourceUuid { get; set; } = "";

	[JsonPropertyName("effect_mtypeid")]
	public int EffectMTypeId { get; set; }

	[JsonPropertyName("effect_stypeid")]
	public int EffectSTypeId { get; set; }

	[JsonPropertyName("tBuy")]
	public int TBuy { get; set; }

	[JsonPropertyName("tExpire")]
	public long TExpire { get; set; }

	[JsonPropertyName("rarity")]
	public string Rarity { get; set; } = "";
}

public class EntityItemPromoteInfo
{
	[JsonPropertyName("begin_at")]
	public int BeginAt { get; set; }

	[JsonPropertyName("end_at")]
	public int EndAt { get; set; }

	[JsonPropertyName("jump_type")]
	public string JumpType { get; set; } = "";

	[JsonPropertyName("jump_target")]
	public string JumpTarget { get; set; } = "";

	[JsonPropertyName("banner_source")]
	public string BannerSource { get; set; } = "";

	[JsonPropertyName("image_url")]
	public string ImageUrl { get; set; } = "";

	[JsonPropertyName("bid")]
	public int Bid { get; set; }
}
