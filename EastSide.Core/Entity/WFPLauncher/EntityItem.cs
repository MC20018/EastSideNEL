using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityNetGameRankList
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("rank_list")] public EntityNetGameRankItem[] RankList { get; set; } = System.Array.Empty<EntityNetGameRankItem>();
}

public class EntityNetGameRankItem
{
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("download_count")] public long DownloadCount { get; set; }
}

public class EntityNetGameBestSellingList
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("item_ids")] public string[] ItemIds { get; set; } = System.Array.Empty<string>();
}

public class EntityNetGameHotList
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("item_ids")] public string[] ItemIds { get; set; } = System.Array.Empty<string>();
}

public class EntityPackage
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("price")] public int Price { get; set; }
	[JsonPropertyName("status")] public int Status { get; set; }
}

public class EntityPackageItem
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("package_id")] public string PackageId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

public class EntityAppearanceItem
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("icon")] public string Icon { get; set; } = string.Empty;
	[JsonPropertyName("type")] public int Type { get; set; }
}
