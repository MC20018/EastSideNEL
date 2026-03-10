using System.Text.Encodings.Web;
using System.Text.Json;
using Codexus.Cipher.Entities;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Room;

public class RoomProtocol : WPFLauncherBase
{
	public RoomProtocol(string version) : base(version)
	{
	}

	#region 房间管理 (源码: age.cs)

	public Entity<LobbyGameRoomEntity> CreateOnlineLobbyRoom(string userId, string userToken,
		string roomName, string resId, uint visibility, uint maxCount, string saveId = "", string password = "")
	{
		return CreateOnlineLobbyRoomAsync(userId, userToken, roomName, resId, visibility, maxCount, saveId, password).GetAwaiter().GetResult();
	}

	private async Task<Entity<LobbyGameRoomEntity>> CreateOnlineLobbyRoomAsync(string userId, string userToken,
		string roomName, string resId, uint visibility, uint maxCount, string saveId, string password)
	{
		string body = JsonSerializer.Serialize(new CreateLobbyGameRoomRequest
		{
			RoomName = roomName, ResId = resId, Visibility = visibility,
			MaxCount = maxCount, SaveId = saveId, Password = password
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<LobbyGameRoomEntity>>(await (await _game.PostAsync("/online-lobby-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<LobbyGameRoomEntity> ResetRoomSetting(string userId, string userToken,
		string roomId, int visibility, bool allowSave, uint ownerId)
	{
		return ResetRoomSettingAsync(userId, userToken, roomId, visibility, allowSave, ownerId).GetAwaiter().GetResult();
	}

	private async Task<Entity<LobbyGameRoomEntity>> ResetRoomSettingAsync(string userId, string userToken,
		string roomId, int visibility, bool allowSave, uint ownerId)
	{
		string body = JsonSerializer.Serialize(new { allow_save = allowSave, visibility, owner_id = ownerId, room_id = roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<LobbyGameRoomEntity>>(await (await _game.PostAsync("/online-lobby-room/update", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<LobbyGameRoomEntity> GetOnlineLobbyRoom(string userId, string userToken, string roomId)
	{
		return GetOnlineLobbyRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<Entity<LobbyGameRoomEntity>> GetOnlineLobbyRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<LobbyGameRoomEntity>>(await (await _game.PostAsync("/online-lobby-room/get", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<LobbyGameRoomEntity> SearchRoomsByResId(string userId, string userToken,
		string resId, int offset = 0, int length = 20)
	{
		return SearchRoomsByResIdAsync(userId, userToken, resId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<LobbyGameRoomEntity>> SearchRoomsByResIdAsync(string userId, string userToken,
		string resId, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new SearchLobbyRoomsByItemIdRequest
		{
			ResId = resId, Offset = offset, Length = length, Version = _version, WithFriend = true
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<LobbyGameRoomEntity>>(await (await _game.PostAsync("/online-lobby-room/query/list-room-by-res-id", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<LobbyGameRoomEntity> SearchRoomsByName(string userId, string userToken,
		string roomName, string resId = "", int offset = 0, int length = 20)
	{
		return SearchRoomsByNameAsync(userId, userToken, roomName, resId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<LobbyGameRoomEntity>> SearchRoomsByNameAsync(string userId, string userToken,
		string roomName, string resId, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new SearchByRoomNameRequest
		{
			RoomName = roomName, ResId = resId, Offset = offset, Length = length, Version = _version
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<LobbyGameRoomEntity>>(await (await _game.PostAsync("/online-lobby-room/query/search-by-name", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse EnterOnlineLobbyRoom(string userId, string userToken,
		string roomId, string password = "", bool checkVisibility = true)
	{
		return EnterOnlineLobbyRoomAsync(userId, userToken, roomId, password, checkVisibility).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> EnterOnlineLobbyRoomAsync(string userId, string userToken,
		string roomId, string password, bool checkVisibility)
	{
		string body = JsonSerializer.Serialize(new LobbyRoomEnterRequest
		{
			RoomId = roomId, Password = password, CheckVisibility = checkVisibility
		}, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/online-lobby-room-enter", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public void LeaveOnlineLobbyRoom(string userId, string userToken, string roomId)
	{
		LeaveOnlineLobbyRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task LeaveOnlineLobbyRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = roomId }, DefaultOptions);
		await _game.PostAsync("/online-lobby-room-enter/leave-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		});
	}

	public void OnlineLobbyMemberTick(string userId, string userToken, string roomId)
	{
		OnlineLobbyMemberTickAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task OnlineLobbyMemberTickAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new LobbyRoomEnterRequest { RoomId = roomId }, DefaultOptions);
		await _game.PostAsync("/online-lobby-member-tick", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		});
	}

	public Entity<LobbyGameAddressEntity> GetOnlineLobbyGameAddress(string userId, string userToken)
	{
		return GetOnlineLobbyGameAddressAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<LobbyGameAddressEntity>> GetOnlineLobbyGameAddressAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<LobbyGameAddressEntity>>(await (await _game.PostAsync("/online-lobby-game-enter", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 成员管理 (源码: agd.cs)

	public Entities<LobbyRoomMemberInfoEntity> GetOnlineLobbyMembers(string userId, string userToken, string roomId)
	{
		return GetOnlineLobbyMembersAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<Entities<LobbyRoomMemberInfoEntity>> GetOnlineLobbyMembersAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<LobbyRoomMemberInfoEntity>>(await (await _game.PostAsync("/online-lobby-member/query/list-by-room-id", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse KickOnlineLobbyMember(string userId, string userToken, string roomId, uint targetUserId)
	{
		return KickOnlineLobbyMemberAsync(userId, userToken, roomId, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> KickOnlineLobbyMemberAsync(string userId, string userToken, string roomId, uint targetUserId)
	{
		string body = JsonSerializer.Serialize(new { room_id = roomId, user_id = targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/online-lobby-member-kick", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 游戏控制 (源码: aga.cs)

	public EntityResponse StartOnlineLobbyGame(string userId, string userToken)
	{
		return OnlineLobbyGameControlAsync(userId, userToken, "start").GetAwaiter().GetResult();
	}

	public EntityResponse StopOnlineLobbyGame(string userId, string userToken)
	{
		return OnlineLobbyGameControlAsync(userId, userToken, "stop").GetAwaiter().GetResult();
	}

	public EntityResponse RestartOnlineLobbyGame(string userId, string userToken)
	{
		return OnlineLobbyGameControlAsync(userId, userToken, "restart").GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> OnlineLobbyGameControlAsync(string userId, string userToken, string action)
	{
		string body = JsonSerializer.Serialize(new { id = action }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/online-lobby-game-control", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 大厅状态 (源码: agg.cs)

	public Entity<OnlineLobbyServerStatusEntity> GetOnlineLobbyStatus(string userId, string userToken)
	{
		return GetOnlineLobbyStatusAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<OnlineLobbyServerStatusEntity>> GetOnlineLobbyStatusAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<OnlineLobbyServerStatusEntity>>(await (await _game.PostAsync("/online-lobby-status", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 收藏 (源码: agc.cs)

	public EntityResponse AddOnlineLobbyCollection(string userId, string userToken, string resId)
	{
		return AddOnlineLobbyCollectionAsync(userId, userToken, resId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> AddOnlineLobbyCollectionAsync(string userId, string userToken, string resId)
	{
		string body = JsonSerializer.Serialize(new { res_id = resId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/online-lobby-collection", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DeleteOnlineLobbyCollection(string userId, string userToken, string entityId)
	{
		return DeleteOnlineLobbyCollectionAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DeleteOnlineLobbyCollectionAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { item_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/online-lobby-collection/delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<LobbyCollectionEntity> GetOnlineLobbyCollections(string userId, string userToken)
	{
		return GetOnlineLobbyCollectionsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<LobbyCollectionEntity>> GetOnlineLobbyCollectionsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<LobbyCollectionEntity>>(await (await _game.PostAsync("/online-lobby-collection", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 备份 (源码: afz.cs)

	public Entities<LobbyGameSaveBackupEntity> GetOnlineLobbyBackups(string userId, string userToken)
	{
		return GetOnlineLobbyBackupsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<LobbyGameSaveBackupEntity>> GetOnlineLobbyBackupsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<LobbyGameSaveBackupEntity>>(await (await _game.PostAsync("/online-lobby-backup/query/list-by-user", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 启动游戏 (源码: afy.cs)

	public EntityResponse LaunchLobbyGame(string userId, string userToken, string roomId)
	{
		return LaunchLobbyGameAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> LaunchLobbyGameAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { room_id = roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/lobby-game", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
