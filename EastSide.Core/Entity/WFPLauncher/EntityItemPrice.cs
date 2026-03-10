using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityItemPrice
{
	[JsonPropertyName("item_id")]
	public string ItemId { get; set; } = "";

	[JsonPropertyName("product_id")]
	public string ProductId { get; set; } = "";

	[JsonPropertyName("level")]
	public int Level { get; set; }

	[JsonPropertyName("expire_time")]
	public int ExpireTime { get; set; }

	[JsonPropertyName("diamonds")]
	public int Diamonds { get; set; }

	[JsonPropertyName("points")]
	public int Points { get; set; }

	[JsonPropertyName("discount")]
	public float Discount { get; set; }

	[JsonPropertyName("vip_discount")]
	public float VipDiscount { get; set; }
}

public class EntityCheckPayLimit
{
	[JsonPropertyName("is_limit")]
	public bool IsLimit { get; set; }

	[JsonPropertyName("create_order_limit")]
	public uint CreateOrderLimit { get; set; }

	[JsonPropertyName("month_sum_limit")]
	public uint MonthSumLimit { get; set; }

	[JsonPropertyName("message")]
	public string Message { get; set; } = "";
}
