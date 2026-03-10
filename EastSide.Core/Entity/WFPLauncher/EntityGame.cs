using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityGameContinue
{
	[JsonPropertyName("game_id")]
	public string GameId { get; set; }

	[JsonPropertyName("game_type")]
	public int GameType { get; set; }

	[JsonPropertyName("address")]
	public string Address { get; set; }

	[JsonPropertyName("port")]
	public int Port { get; set; }

	[JsonPropertyName("token")]
	public string Token { get; set; }
}

public class EntityGameStart
{
	[JsonPropertyName("game_id")]
	public string GameId { get; set; }

	[JsonPropertyName("game_type")]
	public int GameType { get; set; }

	[JsonPropertyName("address")]
	public string Address { get; set; }

	[JsonPropertyName("port")]
	public int Port { get; set; }

	[JsonPropertyName("token")]
	public string Token { get; set; }
}

public class EntityGameEnd
{
	[JsonPropertyName("game_id")]
	public string GameId { get; set; }

	[JsonPropertyName("game_type")]
	public int GameType { get; set; }
}

public class EntityGameStartV2
{
	[JsonPropertyName("game_id")]
	public string GameId { get; set; }

	[JsonPropertyName("game_type")]
	public int GameType { get; set; }

	[JsonPropertyName("address")]
	public string Address { get; set; }

	[JsonPropertyName("port")]
	public int Port { get; set; }

	[JsonPropertyName("token")]
	public string Token { get; set; }

	[JsonPropertyName("uid")]
	public string Uid { get; set; }
}

public class EntityChatBubble
{
	[JsonPropertyName("bubble_id")]
	public int BubbleId { get; set; }
}

public class EntityGroupInfo
{
	[JsonPropertyName("groupid")]
	public string GroupId { get; set; }

	[JsonPropertyName("group_name")]
	public string GroupName { get; set; }

	[JsonPropertyName("img")]
	public string Img { get; set; }

	[JsonPropertyName("ownerid")]
	public string OwnerId { get; set; }

	[JsonPropertyName("post")]
	public int Post { get; set; }

	[JsonPropertyName("invite_perm")]
	public string InvitePerm { get; set; }

	[JsonPropertyName("total")]
	public int Total { get; set; }

	[JsonPropertyName("announcement")]
	public string Announcement { get; set; }
}

public class EntityGroupMember
{
	[JsonPropertyName("uid")]
	public uint Uid { get; set; }

	[JsonPropertyName("post")]
	public int Post { get; set; }
}

public class EntityGroupInviteInfo
{
	[JsonPropertyName("infoid")]
	public string InfoId { get; set; }

	[JsonPropertyName("infotp")]
	public int InfoType { get; set; }

	[JsonPropertyName("groupid")]
	public string GroupId { get; set; }

	[JsonPropertyName("uid")]
	public uint Uid { get; set; }

	[JsonPropertyName("tgt")]
	public uint Target { get; set; }

	[JsonPropertyName("group_name")]
	public string GroupName { get; set; }
}

public class EntityChatSyncPush
{
	[JsonPropertyName("msgs")]
	public Dictionary<string, object> Msgs { get; set; }

	[JsonPropertyName("nextversions")]
	public List<string> NextVersions { get; set; }
}

public class EntityLevelSystem
{
	[JsonPropertyName("nxt_lv")]
	public int NextLevel { get; set; }

	[JsonPropertyName("groupid")]
	public string GroupId { get; set; }
}
