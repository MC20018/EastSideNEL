using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityUserDetail
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";

	[JsonPropertyName("skin")]
	public string Skin { get; set; } = "";

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

	[JsonPropertyName("nickname_init")]
	public int NicknameInit { get; set; }

	[JsonPropertyName("verify_status")]
	public int VerifyStatus { get; set; }

	[JsonPropertyName("user_currency")]
	public EntityUserCurrency? UserCurrency { get; set; }
}

public class EntityUserCurrency
{
	[JsonPropertyName("emerald")]
	public long Emerald { get; set; }

	[JsonPropertyName("diamond")]
	public long Diamond { get; set; }

	[JsonPropertyName("silver")]
	public long Silver { get; set; }
}
