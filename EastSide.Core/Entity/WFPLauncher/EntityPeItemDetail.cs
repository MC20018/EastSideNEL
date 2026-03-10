using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityPeItemDetail
{
	[JsonPropertyName("buy_state")]
	public int BuyState { get; set; }

	[JsonPropertyName("comment_count")]
	public ulong CommentCount { get; set; }

	[JsonPropertyName("developer_id")]
	public ulong DeveloperId { get; set; }

	[JsonPropertyName("developer_name")]
	public string DeveloperName { get; set; } = "";

	[JsonPropertyName("diamond")]
	public double Diamond { get; set; }

	[JsonPropertyName("discount")]
	public double Discount { get; set; }

	[JsonPropertyName("download_num")]
	public string DownloadNum { get; set; } = "";

	[JsonPropertyName("first_type")]
	public int FirstType { get; set; }

	[JsonPropertyName("goods_state")]
	public int GoodsState { get; set; }

	[JsonPropertyName("headimg")]
	public string HeadImg { get; set; } = "";

	[JsonPropertyName("info")]
	public string Info { get; set; } = "";

	[JsonPropertyName("is_item_time_limit")]
	public int IsItemTimeLimit { get; set; }

	[JsonPropertyName("is_sync")]
	public bool IsSync { get; set; }

	[JsonPropertyName("item_id")]
	public string ItemId { get; set; } = "";
	[JsonPropertyName("pic_url_list")]
	public List<string>? PicUrlList { get; set; }

	[JsonPropertyName("points")]
	public double Points { get; set; }

	[JsonPropertyName("res_name")]
	public string ResName { get; set; } = "";

	[JsonPropertyName("res_size")]
	public ulong ResSize { get; set; }

	[JsonPropertyName("title_image_url")]
	public string TitleImageUrl { get; set; } = "";

	[JsonPropertyName("vip_discount")]
	public double VipDiscount { get; set; }

	[JsonPropertyName("vip_only")]
	public bool VipOnly { get; set; }
}
