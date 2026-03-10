using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Chat;

public class ChatProtocol : WPFLauncherBase
{
	public ChatProtocol(string version) : base(version)
	{
	}

	public EntityResponse GetChatMsgs(string userId, string userToken, string targetUserId, long lastMsgId, int length = 20)
	{
		return GetChatMsgsAsync(userId, userToken, targetUserId, lastMsgId, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetChatMsgsAsync(string userId, string userToken, string targetUserId, long lastMsgId, int length)
	{
		string body = JsonSerializer.Serialize(new { targetUserId, lastMsgId, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-chat-msgs", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityChatSyncPush> SyncChatPush(string userId, string userToken, long lastSyncTime)
	{
		return SyncChatPushAsync(userId, userToken, lastSyncTime).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityChatSyncPush>> SyncChatPushAsync(string userId, string userToken, long lastSyncTime)
	{
		string body = JsonSerializer.Serialize(new { lastSyncTime }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityChatSyncPush>>(await (await _game.PostAsync("/sync-chat-push", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse JoinCcRoom(string userId, string userToken, string roomId)
	{
		return JoinCcRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> JoinCcRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-join-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse LeaveCcRoom(string userId, string userToken, string roomId)
	{
		return LeaveCcRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> LeaveCcRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-leave-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CcTalk(string userId, string userToken, string roomId, string audioData)
	{
		return CcTalkAsync(userId, userToken, roomId, audioData).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CcTalkAsync(string userId, string userToken, string roomId, string audioData)
	{
		string body = JsonSerializer.Serialize(new { roomId, audioData }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-talk", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse JoinCcAudio(string userId, string userToken, string roomId)
	{
		return JoinCcAudioAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> JoinCcAudioAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-audio/join", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchCcUsers(string userId, string userToken, string keyword)
	{
		return SearchCcUsersAsync(userId, userToken, keyword).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchCcUsersAsync(string userId, string userToken, string keyword)
	{
		string body = JsonSerializer.Serialize(new { keyword }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-search-users", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetCcPlayerConfig(string userId, string userToken)
	{
		return GetCcPlayerConfigAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetCcPlayerConfigAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc/player/config", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}
}
