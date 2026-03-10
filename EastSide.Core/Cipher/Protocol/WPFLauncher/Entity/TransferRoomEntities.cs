using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;

#region 传统房间实体 (源码: WPFLauncher\Network\Protocol\RoomEntity, ael.cs, aek.cs)

public class TransferRoomEntity
{
	[JsonPropertyName("hid")] public uint Hid { get; set; }
	[JsonPropertyName("rid")] public uint Rid { get; set; }
	[JsonPropertyName("cap")] public uint Cap { get; set; }
	[JsonPropertyName("free")] public uint Free { get; set; }
	[JsonPropertyName("type")] public uint Type { get; set; }
	[JsonPropertyName("name")] public string Name { get; set; }
	[JsonPropertyName("tips")] public string Tips { get; set; }
	[JsonPropertyName("tag_ids")] public List<uint> TagIds { get; set; }
	[JsonPropertyName("srv")] public string Srv { get; set; }
}

public class PeRoomEntity
{
	[JsonPropertyName("hid")] public uint Hid { get; set; }
	[JsonPropertyName("rid")] public uint Rid { get; set; }
	[JsonPropertyName("name")] public string Name { get; set; }
	[JsonPropertyName("type")] public long Type { get; set; }
	[JsonPropertyName("cnt")] public long Cnt { get; set; }
	[JsonPropertyName("cap")] public long Cap { get; set; }
	[JsonPropertyName("srv")] public long Srv { get; set; }
	[JsonPropertyName("platform")] public uint Platform { get; set; }
	[JsonPropertyName("tips")] public string Tips { get; set; }
	[JsonPropertyName("need_password")] public bool NeedPassword { get; set; }
	[JsonPropertyName("tag_ids")] public List<uint> TagIds { get; set; }
	[JsonPropertyName("item_ids")] public List<string> ItemIds { get; set; }
}

public class TransferRoomMemberEntity
{
	[JsonPropertyName("host_id")] public uint HostId { get; set; }
	[JsonPropertyName("room_id")] public uint RoomId { get; set; }
	[JsonPropertyName("room_name")] public string RoomName { get; set; }
	[JsonPropertyName("number")] public int Number { get; set; }
	[JsonPropertyName("max_number")] public int MaxNumber { get; set; }
	[JsonPropertyName("server_addr")] public string ServerAddr { get; set; }
	[JsonPropertyName("nickname")] public string Nickname { get; set; }
	[JsonPropertyName("version")] public uint Version { get; set; }
	[JsonPropertyName("guid")] public string Guid { get; set; }
	[JsonPropertyName("components")] public List<ulong> Components { get; set; }
	[JsonPropertyName("start_server_time")] public ulong StartServerTime { get; set; }
}

#endregion

#region 传统房间响应 (非 EntityResponse 体系，有独立的 code/list 结构)

public class TransferRoomListResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("list")] public List<TransferRoomEntity> List { get; set; }
}

public class TransferSingleRoomResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("info")] public TransferRoomEntity Info { get; set; }
}

public class JavaLanGameRoomResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("list")] public List<TransferRoomEntity> List { get; set; }
}

public class JavaLanGameRoomUserInfoResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("uid_list")] public List<uint> UidList { get; set; }
}

public class PeRoomListResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("list")] public List<PeRoomEntity> List { get; set; }
	[JsonPropertyName("pc_cpp_version")] public int PcCppVersion { get; set; }
	[JsonPropertyName("pc_rtx_version")] public int PcRtxVersion { get; set; }
}

public class PeSingleRoomResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("info")] public PeRoomEntity Info { get; set; }
}

public class TransferCommonResponse
{
	[JsonPropertyName("code")] public int Code { get; set; }
	[JsonPropertyName("message")] public string Message { get; set; }
}

#endregion

#region 传统房间请求

public class SearchRoomByTagQueryRequest
{
	[JsonPropertyName("uid")] public uint Uid { get; set; }
	[JsonPropertyName("tags")] public List<uint> Tags { get; set; }
	[JsonPropertyName("total")] public int Total { get; set; }
}

public class RoomWithNameRequest
{
	[JsonPropertyName("name")] public string Name { get; set; }
}

public class PeRoomWithNameRequest
{
	[JsonPropertyName("name")] public string Name { get; set; }
	[JsonPropertyName("uid")] public uint Uid { get; set; }
}

public class GetSingleRoomRequest
{
	[JsonPropertyName("hid")] public uint Hid { get; set; }
	[JsonPropertyName("rid")] public uint Rid { get; set; }
}

public class GetRoomListRequest
{
	[JsonPropertyName("list")] public List<uint> List { get; set; }
	[JsonPropertyName("total")] public int Total { get; set; }
	[JsonPropertyName("uid")] public uint Uid { get; set; }
}

public class GlobalRoomListRequest
{
	[JsonPropertyName("uid_list")] public List<uint> UidList { get; set; }
}

public class RoomRelatedRequest
{
	[JsonPropertyName("cnt")] public int Cnt { get; set; }
}

public class RoomCollectRequest
{
	[JsonPropertyName("uid")] public uint Uid { get; set; }
	[JsonPropertyName("type")] public int Type { get; set; }
}

public class RoomPasswordCheckRequest
{
	[JsonPropertyName("hid")] public uint Hid { get; set; }
	[JsonPropertyName("rid")] public uint Rid { get; set; }
	[JsonPropertyName("password")] public string Password { get; set; }
}

#endregion
