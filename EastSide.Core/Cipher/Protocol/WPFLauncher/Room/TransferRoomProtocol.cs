using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Room;

public class TransferRoomProtocol : WPFLauncherBase
{
	public TransferRoomProtocol(string version) : base(version, "https://x19transfernew.nie.netease.com")
	{
	}

	#region TransferServer 端点 (源码: ael.cs, adj.c → TransferServerHttpUrl)

	public TransferRoomListResponse GetGlobalRoomList(List<uint> uidList = null)
	{
		return GetGlobalRoomListAsync(uidList).GetAwaiter().GetResult();
	}

	private async Task<TransferRoomListResponse> GetGlobalRoomListAsync(List<uint> uidList)
	{
		string body = JsonSerializer.Serialize(new GlobalRoomListRequest { UidList = uidList }, DefaultOptions);
		return JsonSerializer.Deserialize<TransferRoomListResponse>(await (await _transfer.PostAsync("/global-room-list", body)).Content.ReadAsStringAsync());
	}

	public TransferRoomListResponse GetRelatedRooms(int count)
	{
		return GetRelatedRoomsAsync(count).GetAwaiter().GetResult();
	}

	private async Task<TransferRoomListResponse> GetRelatedRoomsAsync(int count)
	{
		string body = JsonSerializer.Serialize(new RoomRelatedRequest { Cnt = count }, DefaultOptions);
		return JsonSerializer.Deserialize<TransferRoomListResponse>(await (await _transfer.PostAsync("/room-related", body)).Content.ReadAsStringAsync());
	}

	public JavaLanGameRoomResponse SearchRoomsByTags(uint uid, List<uint> tags, int total = 10)
	{
		return SearchRoomsByTagsAsync(uid, tags, total).GetAwaiter().GetResult();
	}

	private async Task<JavaLanGameRoomResponse> SearchRoomsByTagsAsync(uint uid, List<uint> tags, int total)
	{
		string body = JsonSerializer.Serialize(new SearchRoomByTagQueryRequest { Uid = uid, Tags = tags, Total = total }, DefaultOptions);
		return JsonSerializer.Deserialize<JavaLanGameRoomResponse>(await (await _transfer.PostAsync("/room-with-tags", body)).Content.ReadAsStringAsync());
	}

	public TransferRoomListResponse SearchRoomByName(string name)
	{
		return SearchRoomByNameAsync(name).GetAwaiter().GetResult();
	}

	private async Task<TransferRoomListResponse> SearchRoomByNameAsync(string name)
	{
		string body = JsonSerializer.Serialize(new RoomWithNameRequest { Name = name }, DefaultOptions);
		return JsonSerializer.Deserialize<TransferRoomListResponse>(await (await _transfer.PostAsync("/room-with-name", body)).Content.ReadAsStringAsync());
	}

	public TransferSingleRoomResponse GetSingleRoomInfo(uint hid, uint rid)
	{
		return GetSingleRoomInfoAsync(hid, rid).GetAwaiter().GetResult();
	}

	private async Task<TransferSingleRoomResponse> GetSingleRoomInfoAsync(uint hid, uint rid)
	{
		string body = JsonSerializer.Serialize(new GetSingleRoomRequest { Hid = hid, Rid = rid }, DefaultOptions);
		return JsonSerializer.Deserialize<TransferSingleRoomResponse>(await (await _transfer.PostAsync("/single-room-info", body)).Content.ReadAsStringAsync());
	}

	public JavaLanGameRoomUserInfoResponse GetSingleRoomMember(ulong roomId, uint userId)
	{
		return GetSingleRoomMemberAsync(roomId, userId).GetAwaiter().GetResult();
	}

	private async Task<JavaLanGameRoomUserInfoResponse> GetSingleRoomMemberAsync(ulong roomId, uint userId)
	{
		string body = JsonSerializer.Serialize(new { rid = userId, hid = roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<JavaLanGameRoomUserInfoResponse>(await (await _transfer.PostAsync("/single-room-member", body)).Content.ReadAsStringAsync());
	}

	public EntityResponse CollectRoom(string host, int port, uint uid, int type)
	{
		return CollectRoomAsync(host, port, uid, type).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CollectRoomAsync(string host, int port, uint uid, int type)
	{
		string body = JsonSerializer.Serialize(new RoomCollectRequest { Uid = uid, Type = type }, DefaultOptions);
		using var tempClient = new HttpWrapper($"http://{host}:{port}");
		return JsonSerializer.Deserialize<EntityResponse>(await (await tempClient.PostAsync("/room-collect", body)).Content.ReadAsStringAsync());
	}

	#endregion

	#region 密码检查 (源码: adg.cs → adg.e)

	public TransferCommonResponse CheckRoomPassword(uint hid, uint rid, string password)
	{
		return CheckRoomPasswordAsync(hid, rid, password).GetAwaiter().GetResult();
	}

	private async Task<TransferCommonResponse> CheckRoomPasswordAsync(uint hid, uint rid, string password)
	{
		string body = JsonSerializer.Serialize(new RoomPasswordCheckRequest
		{
			Hid = hid | 0x80000000, Rid = rid, Password = password
		}, DefaultOptions);
		return JsonSerializer.Deserialize<TransferCommonResponse>(await (await _transfer.PostAsync("/room-check-enter", body)).Content.ReadAsStringAsync());
	}

	#endregion

	#region 角色管理 (源码: aes.cs, 走 ApiGateway, game_type=LAN_GAME=9)

	public Entities<EntityGameCharacter> GetLanGameCharacters(string userId, string userToken, string gameId, int offset = 0, int length = 10)
	{
		return GetLanGameCharactersAsync(userId, userToken, gameId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGameCharacter>> GetLanGameCharactersAsync(string userId, string userToken, string gameId, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length, user_id = userId, game_id = gameId, game_type = 9 }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/query/user-game-characters", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> CreateLanGameCharacter(string userId, string userToken, string gameId, string name)
	{
		return CreateLanGameCharacterAsync(userId, userToken, gameId, name).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> CreateLanGameCharacterAsync(string userId, string userToken, string gameId, string name)
	{
		string body = JsonSerializer.Serialize(new { entity_id = "0", game_id = gameId, game_type = 9, user_id = userId, name, create_time = 555555 }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> DeleteLanGameCharacter(string userId, string userToken, string entityId)
	{
		return DeleteLanGameCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> DeleteLanGameCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> PreDeleteLanGameCharacter(string userId, string userToken, string entityId)
	{
		return PreDeleteLanGameCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> PreDeleteLanGameCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/pre-delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> CancelPreDeleteLanGameCharacter(string userId, string userToken, string entityId)
	{
		return CancelPreDeleteLanGameCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> CancelPreDeleteLanGameCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/cancel-pre-delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
