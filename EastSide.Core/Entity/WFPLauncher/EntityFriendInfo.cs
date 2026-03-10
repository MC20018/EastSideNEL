using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityFriendInfo
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("fid")]
	public uint Fid { get; set; }

	[JsonPropertyName("uid")]
	public uint Uid { get; set; }

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";

	[JsonPropertyName("head_img")]
	public string HeadImg { get; set; } = "";

	[JsonPropertyName("head_frame")]
	public string HeadFrame { get; set; } = "";

	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("online")]
	public int Online { get; set; }

	[JsonPropertyName("hint")]
	public string Hint { get; set; } = "";

	[JsonPropertyName("gid")]
	public uint Gid { get; set; }

	[JsonPropertyName("mark")]
	public string Mark { get; set; } = "";

	[JsonPropertyName("level")]
	public int Level { get; set; }

	[JsonPropertyName("vip_level")]
	public int VipLevel { get; set; }
}

public class EntitySearchFriend
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";

	[JsonPropertyName("head_img")]
	public string HeadImg { get; set; } = "";

	[JsonPropertyName("head_frame")]
	public string HeadFrame { get; set; } = "";

	[JsonPropertyName("level")]
	public int Level { get; set; }

	[JsonPropertyName("is_friend")]
	public bool IsFriend { get; set; }
}

public class EntityFriendCluster
{
	[JsonPropertyName("gid")]
	public uint Gid { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("count")]
	public int Count { get; set; }
}

public class EntityAddMeFriend
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = "";

	[JsonPropertyName("fid")]
	public uint Fid { get; set; }

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = "";

	[JsonPropertyName("head_img")]
	public string HeadImg { get; set; } = "";

	[JsonPropertyName("message")]
	public string Message { get; set; } = "";

	[JsonPropertyName("time")]
	public long Time { get; set; }
}
