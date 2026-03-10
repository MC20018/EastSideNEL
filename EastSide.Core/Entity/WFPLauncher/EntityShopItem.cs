using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityShopItemType
{
	[JsonPropertyName("gc_item_id")]
	public string GcItemId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("mc_item_count")]
	public uint McItemCount { get; set; }
}

public class EntityShopItem
{
	[JsonPropertyName("gc_item_id")]
	public string GcItemId { get; set; } = "";

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("info")]
	public string Info { get; set; } = "";

	[JsonPropertyName("brief_summary")]
	public string BriefSummary { get; set; } = "";

	[JsonPropertyName("image_url")]
	public string ImageUrl { get; set; } = "";

	[JsonPropertyName("original_diamonds")]
	public uint OriginalDiamonds { get; set; }

	[JsonPropertyName("final_diamonds")]
	public uint FinalDiamonds { get; set; }

	[JsonPropertyName("original_points")]
	public uint OriginalPoints { get; set; }

	[JsonPropertyName("final_points")]
	public uint FinalPoints { get; set; }
}
