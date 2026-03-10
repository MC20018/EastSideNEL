using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityPetSkinInfo
{
	[JsonPropertyName("skin_id")]
	public int SkinId { get; set; }

	[JsonPropertyName("category_id")]
	public int CategoryId { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("icon")]
	public string Icon { get; set; }

	[JsonPropertyName("material")]
	public string Material { get; set; }
}

public class EntityPetDetail
{
	[JsonPropertyName("pet_num")]
	public int PetNum { get; set; }

	[JsonPropertyName("pet_name")]
	public string PetName { get; set; }

	[JsonPropertyName("skin_info")]
	public EntityPetSkinInfo SkinInfo { get; set; }

	[JsonPropertyName("action_ids")]
	public List<int> ActionIds { get; set; }
}

public class EntityHomeShopItemDetail
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("price")]
	public List<int> Price { get; set; }

	[JsonPropertyName("desc")]
	public string Desc { get; set; }

	[JsonPropertyName("rarity")]
	public int Rarity { get; set; }

	[JsonPropertyName("buy_num")]
	public int BuyNum { get; set; }

	[JsonPropertyName("category_id")]
	public int CategoryId { get; set; }
}

public class EntityHomeItemDetail
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("rarity")]
	public int Rarity { get; set; }

	[JsonPropertyName("icon")]
	public string Icon { get; set; }

	[JsonPropertyName("preview_icon")]
	public string PreviewIcon { get; set; }

	[JsonPropertyName("desc")]
	public string Desc { get; set; }

	[JsonPropertyName("category_id")]
	public int CategoryId { get; set; }

	[JsonPropertyName("buy_num")]
	public int BuyNum { get; set; }
}

public class EntityPet
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("possessed")]
	public bool Possessed { get; set; }

	[JsonPropertyName("is_using")]
	public bool IsUsing { get; set; }
}

public class EntityHomeShopItem
{
	[JsonPropertyName("item_num")]
	public int ItemNum { get; set; }

	[JsonPropertyName("category_id")]
	public int CategoryId { get; set; }

	[JsonPropertyName("pet_id")]
	public int PetId { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("icon")]
	public string Icon { get; set; }

	[JsonPropertyName("pre_review_video")]
	public string PreReviewVideo { get; set; }

	[JsonPropertyName("desc")]
	public string Desc { get; set; }

	[JsonPropertyName("rarity")]
	public string Rarity { get; set; }

	[JsonPropertyName("is_new")]
	public bool IsNew { get; set; }

	[JsonPropertyName("total_count")]
	public int TotalCount { get; set; }

	[JsonPropertyName("cur_count")]
	public int CurCount { get; set; }

	[JsonPropertyName("promote_discount")]
	public int PromoteDiscount { get; set; }

	[JsonPropertyName("price")]
	public List<int> Price { get; set; }

	[JsonPropertyName("material")]
	public string Material { get; set; }
}

public class EntityHomeShopBuyResult
{
	[JsonPropertyName("is_has_item")]
	public int IsHasItem { get; set; }

	[JsonPropertyName("is_has_extra")]
	public int IsHasExtra { get; set; }

	[JsonPropertyName("home_gold")]
	public string HomeGold { get; set; }

	[JsonPropertyName("extra_info")]
	public Dictionary<string, string> ExtraInfo { get; set; }

	[JsonPropertyName("item_info")]
	public EntityHomeShopItem ItemInfo { get; set; }
}

public class EntityPetSkinCategory
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("icon")]
	public string Icon { get; set; }

	[JsonPropertyName("has_new")]
	public bool HasNew { get; set; }
}

public class EntityUserItemPurchase
{
	[JsonPropertyName("item_id")]
	public string ItemId { get; set; }

	[JsonPropertyName("user_id")]
	public string UserId { get; set; }

	[JsonPropertyName("purchase_time")]
	public long PurchaseTime { get; set; }

	[JsonPropertyName("last_play_time")]
	public long LastPlayTime { get; set; }

	[JsonPropertyName("total_play_time")]
	public long TotalPlayTime { get; set; }

	[JsonPropertyName("expire_time")]
	public long ExpireTime { get; set; }

	[JsonPropertyName("is_favorite")]
	public int IsFavorite { get; set; }
}

public class EntityUserGameSkin
{
	[JsonPropertyName("user_id")]
	public string UserId { get; set; }

	[JsonPropertyName("item_id")]
	public string ItemId { get; set; }

	[JsonPropertyName("purchase_time")]
	public long PurchaseTime { get; set; }

	[JsonPropertyName("last_play_time")]
	public long LastPlayTime { get; set; }

	[JsonPropertyName("total_play_time")]
	public long TotalPlayTime { get; set; }

	[JsonPropertyName("expire_time")]
	public long ExpireTime { get; set; }

	[JsonPropertyName("is_expired")]
	public bool IsExpired { get; set; }
}

public class EntityLocalSkin
{
	[JsonPropertyName("user_id")]
	public string UserId { get; set; }

	[JsonPropertyName("skin_type")]
	public int SkinType { get; set; }

	[JsonPropertyName("skin_slot")]
	public int SkinSlot { get; set; }

	[JsonPropertyName("md5")]
	public string Md5 { get; set; }

	[JsonPropertyName("size")]
	public long Size { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("upload_time")]
	public long UploadTime { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; }
}

public class EntityReviewStatus
{
	[JsonPropertyName("review_status")]
	public int ReviewStatus { get; set; }
}
