using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Group;

public class GroupProtocol : WPFLauncherBase
{
	public GroupProtocol(string version) : base(version)
	{
	}

	public Entity<EntityGroupInfo> CreateGroup(string userId, string userToken, string groupName, string groupDesc = "")
	{
		return CreateGroupAsync(userId, userToken, groupName, groupDesc).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGroupInfo>> CreateGroupAsync(string userId, string userToken, string groupName, string groupDesc)
	{
		string body = JsonSerializer.Serialize(new { groupName, groupDesc }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGroupInfo>>(await (await _game.PostAsync("/create-group", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGroupInfo> UserCreateGroup(string userId, string userToken, string groupName, string groupDesc = "")
	{
		return UserCreateGroupAsync(userId, userToken, groupName, groupDesc).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGroupInfo>> UserCreateGroupAsync(string userId, string userToken, string groupName, string groupDesc)
	{
		string body = JsonSerializer.Serialize(new { groupName, groupDesc }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGroupInfo>>(await (await _game.PostAsync("/user-create-group", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityGroupInfo> GetAllGroups(string userId, string userToken)
	{
		return GetAllGroupsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGroupInfo>> GetAllGroupsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityGroupInfo>>(await (await _game.PostAsync("/get-all-groups", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityGroupInfo> GetGroups(string userId, string userToken, int offset = 0, int length = 20)
	{
		return GetGroupsAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGroupInfo>> GetGroupsAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGroupInfo>>(await (await _game.PostAsync("/get-groups", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityGroupInfo> GetGroupInfos(string userId, string userToken, List<string> groupIds)
	{
		return GetGroupInfosAsync(userId, userToken, groupIds).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGroupInfo>> GetGroupInfosAsync(string userId, string userToken, List<string> groupIds)
	{
		string body = JsonSerializer.Serialize(new { groupIds }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGroupInfo>>(await (await _game.PostAsync("/get-group-infos", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityGroupMember> GetGroupMember(string userId, string userToken, string groupId)
	{
		return GetGroupMemberAsync(userId, userToken, groupId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGroupMember>> GetGroupMemberAsync(string userId, string userToken, string groupId)
	{
		string body = JsonSerializer.Serialize(new { groupId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGroupMember>>(await (await _game.PostAsync("/get-group-member", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GroupInvite(string userId, string userToken, string groupId, string targetUserId)
	{
		return GroupInviteAsync(userId, userToken, groupId, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GroupInviteAsync(string userId, string userToken, string groupId, string targetUserId)
	{
		string body = JsonSerializer.Serialize(new { groupId, targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/group-invite", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ReplyGroupInvite(string userId, string userToken, string groupId, int replyType)
	{
		return ReplyGroupInviteAsync(userId, userToken, groupId, replyType).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ReplyGroupInviteAsync(string userId, string userToken, string groupId, int replyType)
	{
		string body = JsonSerializer.Serialize(new { groupId, replyType }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/reply-group-invite", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GroupKickout(string userId, string userToken, string groupId, string targetUserId)
	{
		return GroupKickoutAsync(userId, userToken, groupId, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GroupKickoutAsync(string userId, string userToken, string groupId, string targetUserId)
	{
		string body = JsonSerializer.Serialize(new { groupId, targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/group-kickout", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse QuitGroup(string userId, string userToken, string groupId)
	{
		return QuitGroupAsync(userId, userToken, groupId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> QuitGroupAsync(string userId, string userToken, string groupId)
	{
		string body = JsonSerializer.Serialize(new { groupId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/quit-group", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UserDeleteGroup(string userId, string userToken, string groupId)
	{
		return UserDeleteGroupAsync(userId, userToken, groupId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UserDeleteGroupAsync(string userId, string userToken, string groupId)
	{
		string body = JsonSerializer.Serialize(new { groupId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-delete-group", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UserRenameGroup(string userId, string userToken, string groupId, string newName)
	{
		return UserRenameGroupAsync(userId, userToken, groupId, newName).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UserRenameGroupAsync(string userId, string userToken, string groupId, string newName)
	{
		string body = JsonSerializer.Serialize(new { groupId, newName }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-rename-group", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeGroupName(string userId, string userToken, string groupId, string newName)
	{
		return ChangeGroupNameAsync(userId, userToken, groupId, newName).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeGroupNameAsync(string userId, string userToken, string groupId, string newName)
	{
		string body = JsonSerializer.Serialize(new { groupId, newName }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/change-group-name", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeGroupImg(string userId, string userToken, string groupId, string imgUrl)
	{
		return ChangeGroupImgAsync(userId, userToken, groupId, imgUrl).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeGroupImgAsync(string userId, string userToken, string groupId, string imgUrl)
	{
		string body = JsonSerializer.Serialize(new { groupId, imgUrl }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/change-group-img", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeGroupInvitePerm(string userId, string userToken, string groupId, int permType)
	{
		return ChangeGroupInvitePermAsync(userId, userToken, groupId, permType).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeGroupInvitePermAsync(string userId, string userToken, string groupId, int permType)
	{
		string body = JsonSerializer.Serialize(new { groupId, permType }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/change-group-invite-perm", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ChangeGroupPost(string userId, string userToken, string groupId, string postId, int action)
	{
		return ChangeGroupPostAsync(userId, userToken, groupId, postId, action).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ChangeGroupPostAsync(string userId, string userToken, string groupId, string postId, int action)
	{
		string body = JsonSerializer.Serialize(new { groupId, postId, action }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/change-group-post", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetMoreGroupMsgs(string userId, string userToken, string groupId, long lastMsgId, int length = 20)
	{
		return GetMoreGroupMsgsAsync(userId, userToken, groupId, lastMsgId, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetMoreGroupMsgsAsync(string userId, string userToken, string groupId, long lastMsgId, int length)
	{
		string body = JsonSerializer.Serialize(new { groupId, lastMsgId, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-more-group-msgs", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}
}
