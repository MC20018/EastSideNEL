using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Other;

public class OtherProtocol : WPFLauncherBase
{
	public OtherProtocol(string version) : base(version)
	{
	}

	#region 账单和货币

	public EntityResponse GetBill(string userId, string userToken, string billId)
	{
		return GetBillAsync(userId, userToken, billId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetBillAsync(string userId, string userToken, string billId)
	{
		string body = JsonSerializer.Serialize(new { billId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/bill/get", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityUserCurrency> GetUserCurrency(string userId, string userToken)
	{
		return GetUserCurrencyAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityUserCurrency>> GetUserCurrencyAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityUserCurrency>>(await (await _game.PostAsync("/user-currency/query", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetCurrencyOnline(string userId, string userToken)
	{
		return GetCurrencyOnlineAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetCurrencyOnlineAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-currency-online", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 用户集群和动态

	public EntityResponse GetUserCluster(string userId, string userToken)
	{
		return GetUserClusterAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserClusterAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-cluster", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetUserMoment(string userId, string userToken, string targetUserId, int offset = 0, int length = 20)
	{
		return GetUserMomentAsync(userId, userToken, targetUserId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserMomentAsync(string userId, string userToken, string targetUserId, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { targetUserId, offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-moment", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 个人信息

	public Entity<EntityPersonalInfo> GetPersonalInfo(string userId, string userToken)
	{
		return GetPersonalInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityPersonalInfo>> GetPersonalInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityPersonalInfo>>(await (await _game.PostAsync("/personal-info/get", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CheckNickname(string userId, string userToken, string nickname)
	{
		return CheckNicknameAsync(userId, userToken, nickname).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CheckNicknameAsync(string userId, string userToken, string nickname)
	{
		string body = JsonSerializer.Serialize(new { nickname }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/personal-info/check-nickname", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetPageUrl(string userId, string userToken, string pageId)
	{
		return GetPageUrlAsync(userId, userToken, pageId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetPageUrlAsync(string userId, string userToken, string pageId)
	{
		string body = JsonSerializer.Serialize(new { pageId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-page-url", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 设置和上传

	public EntityResponse GetUserSetting(string userId, string userToken)
	{
		return GetUserSettingAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserSettingAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-user-setting", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetUserSetting(string userId, string userToken, string settingKey, string settingValue)
	{
		return SetUserSettingAsync(userId, userToken, settingKey, settingValue).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetUserSettingAsync(string userId, string userToken, string settingKey, string settingValue)
	{
		string body = JsonSerializer.Serialize(new { settingKey, settingValue }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/set-user-setting", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetImageUploadToken(string userId, string userToken)
	{
		return GetImageUploadTokenAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetImageUploadTokenAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/image-upload-token", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse RequestSpriteUrlToken(string userId, string userToken)
	{
		return RequestSpriteUrlTokenAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> RequestSpriteUrlTokenAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/request-spriteurl-token", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 令牌和日志

	public EntityResponse GetCustomerServiceToken(string userId, string userToken)
	{
		return GetCustomerServiceTokenAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetCustomerServiceTokenAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/customer-service-token", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetAladingWebToken(string userId, string userToken)
	{
		return GetAladingWebTokenAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetAladingWebTokenAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/alading-web-token", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetForumToken(string userId, string userToken)
	{
		return GetForumTokenAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetForumTokenAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/forum-token/", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UploadClientLog(string userId, string userToken, string logData)
	{
		return UploadClientLogAsync(userId, userToken, logData).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UploadClientLogAsync(string userId, string userToken, string logData)
	{
		string body = JsonSerializer.Serialize(new { logData }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/client-log", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UpdateInstructionState(string userId, string userToken, string instructionId, int state)
	{
		return UpdateInstructionStateAsync(userId, userToken, instructionId, state).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UpdateInstructionStateAsync(string userId, string userToken, string instructionId, int state)
	{
		string body = JsonSerializer.Serialize(new { instructionId, state }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/instruction-state/update", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 等级系统和CC

	public Entity<EntityLevelSystem> GetLevelSystem(string userId, string userToken)
	{
		return GetLevelSystemAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityLevelSystem>> GetLevelSystemAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityLevelSystem>>(await (await _game.PostAsync("/level-system/get-detail", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UnlockLevelSystem(string userId, string userToken, string unlockId)
	{
		return UnlockLevelSystemAsync(userId, userToken, unlockId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UnlockLevelSystemAsync(string userId, string userToken, string unlockId)
	{
		string body = JsonSerializer.Serialize(new { unlockId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/level-system/unlock/", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CreateLevelSystemScoreEvent(string userId, string userToken, int score, string eventType)
	{
		return CreateLevelSystemScoreEventAsync(userId, userToken, score, eventType).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CreateLevelSystemScoreEventAsync(string userId, string userToken, int score, string eventType)
	{
		string body = JsonSerializer.Serialize(new { score, eventType }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/level-system/score-event/create", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetLevelSystemValueTable(string userId, string userToken)
	{
		return GetLevelSystemValueTableAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetLevelSystemValueTableAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/level-system/value-table", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CcAudioJoin(string userId, string userToken, string roomId)
	{
		return CcAudioJoinAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CcAudioJoinAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-audio/join", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CcTalk(string userId, string userToken, string targetUserId)
	{
		return CcTalkAsync(userId, userToken, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CcTalkAsync(string userId, string userToken, string targetUserId)
	{
		string body = JsonSerializer.Serialize(new { targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-talk", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CcJoinRoom(string userId, string userToken, string roomId)
	{
		return CcJoinRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CcJoinRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-join-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CcLeaveRoom(string userId, string userToken, string roomId)
	{
		return CcLeaveRoomAsync(userId, userToken, roomId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CcLeaveRoomAsync(string userId, string userToken, string roomId)
	{
		string body = JsonSerializer.Serialize(new { roomId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/cc-leave-room", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 其他查询

	public EntityResponse SearchUserGamePurchaseByIds(string userId, string userToken, List<string> purchaseIds)
	{
		return SearchUserGamePurchaseByIdsAsync(userId, userToken, purchaseIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchUserGamePurchaseByIdsAsync(string userId, string userToken, List<string> purchaseIds)
	{
		string body = JsonSerializer.Serialize(new { purchaseIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-game-purchase/query/search-by-ids", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchUserGamePurchaseByUser(string userId, string userToken)
	{
		return SearchUserGamePurchaseByUserAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchUserGamePurchaseByUserAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-game-purchase/query/search-by-user", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetPeRealnameV2Status(string userId, string userToken)
	{
		return GetPeRealnameV2StatusAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetPeRealnameV2StatusAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/pe-realname-v2/status", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchGameAuthItemListByGame(string userId, string userToken, string gameId)
	{
		return SearchGameAuthItemListByGameAsync(userId, userToken, gameId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchGameAuthItemListByGameAsync(string userId, string userToken, string gameId)
	{
		string body = JsonSerializer.Serialize(new { gameId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/game-auth-item-list/query/search-by-game", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetPcNetGameAddressList(string userId, string userToken)
	{
		return GetPcNetGameAddressListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetPcNetGameAddressListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/pc/web/net-game-address/get-address-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetAddressList(string userId, string userToken)
	{
		return GetAddressListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetAddressListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-address-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
