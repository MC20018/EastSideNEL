using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.User;

public class UserProtocol : WPFLauncherBase
{
	public UserProtocol(string gameVersion) : base(gameVersion) { }

	public UserProtocol(HttpWrapper client, HttpWrapper core, HttpWrapper game, HttpWrapper gateway, HttpWrapper rental)
		: base(client, core, game, gateway, rental) { }

	public Entity<EntityUserDetail> GetUserDetail(string userId, string userToken)
	{
		return GetUserDetailAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityUserDetail>> GetUserDetailAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityUserDetail>>(await (await _game.PostAsync("/user-detail", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetUserHead(string userId, string userToken, string headUrl)
	{
		return SetUserHeadAsync(userId, userToken, headUrl).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetUserHeadAsync(string userId, string userToken, string headUrl)
	{
		string body = JsonSerializer.Serialize(new { head_url = headUrl }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-head", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetUserHeadFrame(string userId, string userToken, string frameId)
	{
		return SetUserHeadFrameAsync(userId, userToken, frameId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetUserHeadFrameAsync(string userId, string userToken, string frameId)
	{
		string body = JsonSerializer.Serialize(new { frame_id = frameId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-head-frame", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityUserDetail> GetUserProfile(string userId, string userToken, string targetUserId)
	{
		return GetUserProfileAsync(userId, userToken, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityUserDetail>> GetUserProfileAsync(string userId, string userToken, string targetUserId)
	{
		string body = JsonSerializer.Serialize(new { target_user_id = targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityUserDetail>>(await (await _game.PostAsync("/user/profile", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityUserDetail> SearchUsersByIds(string userId, string userToken, string[] userIds)
	{
		return SearchUsersByIdsAsync(userId, userToken, userIds).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityUserDetail>> SearchUsersByIdsAsync(string userId, string userToken, string[] userIds)
	{
		string body = JsonSerializer.Serialize(new { user_ids = userIds }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityUserDetail>>(await (await _game.PostAsync("/user/query/search-by-ids", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntitySearchFriend> SearchUserByUid(string userId, string userToken, string targetUid)
	{
		return SearchUserByUidAsync(userId, userToken, targetUid).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntitySearchFriend>> SearchUserByUidAsync(string userId, string userToken, string targetUid)
	{
		string body = JsonSerializer.Serialize(new { uid = targetUid }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntitySearchFriend>>(await (await _game.PostAsync("/user/query/search-by-uid", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityPersonalInfo> GetPersonalInfo(string userId, string userToken)
	{
		return GetPersonalInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityPersonalInfo>> GetPersonalInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityPersonalInfo>>(await (await _game.PostAsync("/personal-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
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

	public EntityResponse InitNickname(string userId, string userToken, string nickname)
	{
		return InitNicknameAsync(userId, userToken, nickname).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> InitNicknameAsync(string userId, string userToken, string nickname)
	{
		string body = JsonSerializer.Serialize(new { nickname }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/nickname-init", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetNickname(string userId, string userToken, string nickname)
	{
		return SetNicknameAsync(userId, userToken, nickname).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetNicknameAsync(string userId, string userToken, string nickname)
	{
		string body = JsonSerializer.Serialize(new { nickname }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/nickname-setting", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityReviewStatus> GetReviewStatus(string userId, string userToken)
	{
		return GetReviewStatusAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityReviewStatus>> GetReviewStatusAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityReviewStatus>>(await (await _game.PostAsync("/review_status", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityFriendInfo> GetAllFriends(string userId, string userToken)
	{
		return GetAllFriendsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityFriendInfo>> GetAllFriendsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityFriendInfo>>(await (await _game.PostAsync("/user-allfriends", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityFriendInfo> GetAllFriendsWithDetail(string userId, string userToken)
	{
		return GetAllFriendsWithDetailAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityFriendInfo>> GetAllFriendsWithDetailAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityFriendInfo>>(await (await _game.PostAsync("/user-allfriends-with-detail", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityAddMeFriend> GetAddMeFriends(string userId, string userToken)
	{
		return GetAddMeFriendsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityAddMeFriend>> GetAddMeFriendsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityAddMeFriend>>(await (await _game.PostAsync("/user-addme-friends", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ApplyFriend(string userId, string userToken, string friendId)
	{
		return ApplyFriendAsync(userId, userToken, friendId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ApplyFriendAsync(string userId, string userToken, string friendId)
	{
		string body = JsonSerializer.Serialize(new { friend_id = friendId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-apply-friend", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ReplyFriend(string userId, string userToken, string friendId, int reply)
	{
		return ReplyFriendAsync(userId, userToken, friendId, reply).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ReplyFriendAsync(string userId, string userToken, string friendId, int reply)
	{
		string body = JsonSerializer.Serialize(new { friend_id = friendId, reply }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-reply-friend", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DeleteFriend(string userId, string userToken, string friendId)
	{
		return DeleteFriendAsync(userId, userToken, friendId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DeleteFriendAsync(string userId, string userToken, string friendId)
	{
		string body = JsonSerializer.Serialize(new { friend_id = friendId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-del-friend", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeFriendMark(string userId, string userToken, string friendId, string mark)
	{
		return ChangeFriendMarkAsync(userId, userToken, friendId, mark).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeFriendMarkAsync(string userId, string userToken, string friendId, string mark)
	{
		string body = JsonSerializer.Serialize(new { friend_id = friendId, mark }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-change-friend-mark", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntitySearchFriend> SearchFriend(string userId, string userToken, string keyword)
	{
		return SearchFriendAsync(userId, userToken, keyword).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntitySearchFriend>> SearchFriendAsync(string userId, string userToken, string keyword)
	{
		string body = JsonSerializer.Serialize(new { keyword }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntitySearchFriend>>(await (await _game.PostAsync("/user-search-friend", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetFriendOnlineStatus(string userId, string userToken)
	{
		return GetFriendOnlineStatusAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetFriendOnlineStatusAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-friend-online-status", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeUserStatus(string userId, string userToken, int status)
	{
		return ChangeUserStatusAsync(userId, userToken, status).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeUserStatusAsync(string userId, string userToken, int status)
	{
		string body = JsonSerializer.Serialize(new { status }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-change-status", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
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
		return JsonSerializer.Deserialize<Entity<EntityUserCurrency>>(await (await _game.PostAsync("/user-currency", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetUserNotifyCount(string userId, string userToken)
	{
		return GetUserNotifyCountAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserNotifyCountAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-notify-count", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse RealnameAuth(string userId, string userToken, string realName, string idCard)
	{
		return RealnameAuthAsync(userId, userToken, realName, idCard).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> RealnameAuthAsync(string userId, string userToken, string realName, string idCard)
	{
		string body = JsonSerializer.Serialize(new { real_name = realName, id_card = idCard }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-realname-auth", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse RefreshRealname(string userId, string userToken)
	{
		return RefreshRealnameAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> RefreshRealnameAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-refresh-realname", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse MobileBindQuery(string userId, string userToken)
	{
		return MobileBindQueryAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> MobileBindQueryAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/mobile-bind-query", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse MobileBindSendSms(string userId, string userToken, string mobile)
	{
		return MobileBindSendSmsAsync(userId, userToken, mobile).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> MobileBindSendSmsAsync(string userId, string userToken, string mobile)
	{
		string body = JsonSerializer.Serialize(new { mobile }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/mobile-bind-send-sms", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse MobileBindUpdate(string userId, string userToken, string mobile, string code)
	{
		return MobileBindUpdateAsync(userId, userToken, mobile, code).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> MobileBindUpdateAsync(string userId, string userToken, string mobile, string code)
	{
		string body = JsonSerializer.Serialize(new { mobile, code }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/mobile-bind-update", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}
}
