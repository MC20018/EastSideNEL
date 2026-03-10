using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityVipGrowthInfo
{
	[JsonPropertyName("vip_level")]
	public int VipLevel { get; set; }

	[JsonPropertyName("growth_value")]
	public long GrowthValue { get; set; }

	[JsonPropertyName("next_level_growth")]
	public long NextLevelGrowth { get; set; }

	[JsonPropertyName("is_vip")]
	public bool IsVip { get; set; }

	[JsonPropertyName("vip_expire_time")]
	public long VipExpireTime { get; set; }

	[JsonPropertyName("vip_type")]
	public int VipType { get; set; }
}
