using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Activity;

public class ActivityProtocol : WPFLauncherBase
{
	public ActivityProtocol(string version) : base(version)
	{
	}

	#region 活动 (源码: afi.cs)

	public Entity<EntityActivityStateFlag> GetActivityStateFlag(string userId, string userToken, string activityId)
	{
		return GetActivityStateFlagAsync(userId, userToken, activityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityActivityStateFlag>> GetActivityStateFlagAsync(string userId, string userToken, string activityId)
	{
		string body = JsonSerializer.Serialize(new { activityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityActivityStateFlag>>(await (await _game.PostAsync("/activity-get-state-flag", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityActivityDate> GetActivityDate(string userId, string userToken, string activityId)
	{
		return GetActivityDateAsync(userId, userToken, activityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityActivityDate>> GetActivityDateAsync(string userId, string userToken, string activityId)
	{
		string body = JsonSerializer.Serialize(new { activityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityActivityDate>>(await (await _game.PostAsync("/activity-get-date", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityActivityInfo> GetAllActivityInfo(string userId, string userToken)
	{
		return GetAllActivityInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityActivityInfo>> GetAllActivityInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityActivityInfo>>(await (await _game.PostAsync("/interconn/web/activity/get-all-activity-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCheckActivity> FirstChargeQuery(string userId, string userToken)
	{
		return FirstChargeQueryAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCheckActivity>> FirstChargeQueryAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityCheckActivity>>(await (await _game.PostAsync("/interconn/web/charge-activity/first-charge-query", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityChargeActivityInfo> DiamondActivityInfoQuery(string userId, string userToken, string activityType)
	{
		return DiamondActivityInfoQueryAsync(userId, userToken, activityType).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityChargeActivityInfo>> DiamondActivityInfoQueryAsync(string userId, string userToken, string activityType)
	{
		string body = JsonSerializer.Serialize(new { type = activityType }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityChargeActivityInfo>>(await (await _game.PostAsync("/interconn/web/diamond-activity/activity-info-query", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityChargeActivityReward> GetAllRewards(string userId, string userToken, string activityType, string[] rewardIds)
	{
		return GetAllRewardsAsync(userId, userToken, activityType, rewardIds).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityChargeActivityReward>> GetAllRewardsAsync(string userId, string userToken, string activityType, string[] rewardIds)
	{
		string body = JsonSerializer.Serialize(new { type = activityType, reward_ids = rewardIds }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityChargeActivityReward>>(await (await _game.PostAsync("/interconn/web/diamond-activity/get-all-rewards", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityNextActivityInfo> GetNextRewardInfo(string userId, string userToken, string activityType)
	{
		return GetNextRewardInfoAsync(userId, userToken, activityType).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityNextActivityInfo>> GetNextRewardInfoAsync(string userId, string userToken, string activityType)
	{
		string body = JsonSerializer.Serialize(new { type = activityType }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityNextActivityInfo>>(await (await _game.PostAsync("/interconn/web/diamond-activity/get-next-reward-info", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityLibaoItemInfo> GetLibaoUserItemInfo(string userId, string userToken, int receiverId, string itemId, string activityName)
	{
		return GetLibaoUserItemInfoAsync(userId, userToken, receiverId, itemId, activityName).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityLibaoItemInfo>> GetLibaoUserItemInfoAsync(string userId, string userToken, int receiverId, string itemId, string activityName)
	{
		string body = JsonSerializer.Serialize(new { receiver_id = receiverId, item_id = itemId, activity_name = activityName }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityLibaoItemInfo>>(await (await _game.PostAsync("/activities/libao/get-user-item-info", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetActivityImgUrl(string userId, string userToken, string activityId)
	{
		return GetActivityImgUrlAsync(userId, userToken, activityId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetActivityImgUrlAsync(string userId, string userToken, string activityId)
	{
		string body = JsonSerializer.Serialize(new { activityId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-activity-imgurl", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 折扣包 (源码: ads.cs)

	public Entity<EntityDiscountPackInfo> GetDiscountPackInfo(string userId, string userToken)
	{
		return GetDiscountPackInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityDiscountPackInfo>> GetDiscountPackInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityDiscountPackInfo>>(await (await _game.PostAsync("/interconn/web/activity/discount-pack-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityDiscountPackPush> GetDiscountPackPush(string userId, string userToken)
	{
		return GetDiscountPackPushAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityDiscountPackPush>> GetDiscountPackPushAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityDiscountPackPush>>(await (await _game.PostAsync("/interconn/web/activity/discount-pack-push", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DiscountPackBuyPayBack(string userId, string userToken, string packId)
	{
		return DiscountPackBuyPayBackAsync(userId, userToken, packId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DiscountPackBuyPayBackAsync(string userId, string userToken, string packId)
	{
		string body = JsonSerializer.Serialize(new { packId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/activity/discount-pack-buy-pay_back", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 每日签到 (源码: adt.cs)

	public Entity<EntityDailySignState> GetDailySignState(string userId, string userToken)
	{
		return GetDailySignStateAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityDailySignState>> GetDailySignStateAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityDailySignState>>(await (await _game.PostAsync("/interconn/web/interactivity/daily-sign-state", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntitySignReward> GetDailyReward(string userId, string userToken)
	{
		return GetDailyRewardAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntitySignReward>> GetDailyRewardAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntitySignReward>>(await (await _game.PostAsync("/interconn/web/interactivity/get-daily-reward", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 签到/蜂巢 (源码: aec.cs)

	public Entities<EntitySignIn> SignIn(string userId, string userToken)
	{
		return SignInAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntitySignIn>> SignInAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntitySignIn>>(await (await _game.PostAsync("/interconn/web/sign/sign-in", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse ReplacementBuy(string userId, string userToken)
	{
		return ReplacementBuyAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> ReplacementBuyAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/sign/replacement-buy", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DailyQuestGet(string userId, string userToken)
	{
		return DailyQuestGetAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DailyQuestGetAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/daily-quest/daily-quest-get", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DailyRewardExchange(string userId, string userToken)
	{
		return DailyRewardExchangeAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DailyRewardExchangeAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/daily-quest/daily-reward-exchange", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region VIP

	public EntityResponse GetVip(string userId, string userToken)
	{
		return GetVipAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetVipAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse BuyVip(string userId, string userToken, string mealId)
	{
		return BuyVipAsync(userId, userToken, mealId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> BuyVipAsync(string userId, string userToken, string mealId)
	{
		string body = JsonSerializer.Serialize(new { mealId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip/buy", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SendVip(string userId, string userToken, string mealId, string receiverId)
	{
		return SendVipAsync(userId, userToken, mealId, receiverId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SendVipAsync(string userId, string userToken, string mealId, string receiverId)
	{
		string body = JsonSerializer.Serialize(new { mealId, receiverId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip/send", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityVipGrowthInfo> GetVipGrowthInfo(string userId, string userToken)
	{
		return GetVipGrowthInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityVipGrowthInfo>> GetVipGrowthInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityVipGrowthInfo>>(await (await _game.PostAsync("/vip/growth/get-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetBenefits(string userId, string userToken)
	{
		return GetBenefitsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetBenefitsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip/benefits", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetSecondaryItemTypes(string userId, string userToken)
	{
		return GetSecondaryItemTypesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetSecondaryItemTypesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip/benefits/secondary-item-types", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetSeasonItems(string userId, string userToken)
	{
		return GetSeasonItemsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetSeasonItemsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/vip/season/items", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetOptionalPackInfo(string userId, string userToken)
	{
		return GetOptionalPackInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetOptionalPackInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-optional-pack-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 优惠券和试用

	public EntityResponse GetAvailableCouponForItem(string userId, string userToken, string itemId)
	{
		return GetAvailableCouponForItemAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetAvailableCouponForItemAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/coupon/available-for-item", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetAvailableCouponV2(string userId, string userToken)
	{
		return GetAvailableCouponV2Async(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetAvailableCouponV2Async(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/coupon/availableV2", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetTrailList(string userId, string userToken)
	{
		return GetTrailListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetTrailListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/trail/get-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UseTrailTicket(string userId, string userToken, string ticketId, string itemId)
	{
		return UseTrailTicketAsync(userId, userToken, ticketId, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UseTrailTicketAsync(string userId, string userToken, string ticketId, string itemId)
	{
		string body = JsonSerializer.Serialize(new { ticketId, itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/trail/use-ticket", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
