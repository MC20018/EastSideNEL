using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityPersonalInfo
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";

	[JsonPropertyName("head_img")]
	public string HeadImg { get; set; } = "";

	[JsonPropertyName("head_frame")]
	public string HeadFrame { get; set; } = "";

	[JsonPropertyName("gender")]
	public int Gender { get; set; }

	[JsonPropertyName("level")]
	public int Level { get; set; }

	[JsonPropertyName("vip_level")]
	public int VipLevel { get; set; }

	[JsonPropertyName("is_vip")]
	public bool IsVip { get; set; }

	[JsonPropertyName("skin")]
	public string Skin { get; set; } = "";
}

public class EntityDailySignState
{
	[JsonPropertyName("today_sign_status")]
	public bool TodaySignStatus { get; set; }

	[JsonPropertyName("total_sign")]
	public int TotalSign { get; set; }

	[JsonPropertyName("begin_time")]
	public long BeginTime { get; set; }

	[JsonPropertyName("end_time")]
	public long EndTime { get; set; }

	[JsonPropertyName("background_url")]
	public string BackgroundUrl { get; set; } = "";

	[JsonPropertyName("title_url")]
	public string TitleUrl { get; set; } = "";
}

public class EntityNicknameReviewStatus
{
	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";
}
