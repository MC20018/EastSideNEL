using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using EastSide.Core.Entity.WFPLauncher;
using EastSide.Core.Entity.WFPLauncher.NetGame;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Item;

public class ItemProtocol : WPFLauncherBase
{
	public ItemProtocol(string version) : base(version)
	{
	}

	#region 物品查询 (源码: adr.cs)

	public Entity<EntityResourceItem> GetAvailableItems(string userId, string userToken, int offset = 0, int length = 20)
	{
		return GetAvailableItemsAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> GetAvailableItemsAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/available", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> SearchItemsByIds(string userId, string userToken, List<string> itemIds)
	{
		return SearchItemsByIdsAsync(userId, userToken, itemIds).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> SearchItemsByIdsAsync(string userId, string userToken, List<string> itemIds)
	{
		string body = JsonSerializer.Serialize(new { item_ids = itemIds }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/search-by-ids", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> SearchItemsByKeyword(string userId, string userToken, string keyword)
	{
		return SearchItemsByKeywordAsync(userId, userToken, keyword).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> SearchItemsByKeywordAsync(string userId, string userToken, string keyword)
	{
		string body = JsonSerializer.Serialize(new { keyword }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/search-by-keyword", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> GetFriendsPurchasedItems(string userId, string userToken, int offset = 0, int length = 20)
	{
		return GetFriendsPurchasedItemsAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> GetFriendsPurchasedItemsAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/friends-purchased", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityNetGameRankList> GetNewRankingList(string userId, string userToken)
	{
		return GetNewRankingListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityNetGameRankList>> GetNewRankingListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityNetGameRankList>>(await (await _game.PostAsync("/item-download-info/query/new-ranking-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityNetGameBestSellingList> GetBestSellingGames(string userId, string userToken)
	{
		return GetBestSellingGamesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityNetGameBestSellingList>> GetBestSellingGamesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityNetGameBestSellingList>>(await (await _game.PostAsync("/item/query/best-selling-games", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityNetGameHotList> GetRecentHotGames(string userId, string userToken)
	{
		return GetRecentHotGamesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityNetGameHotList>> GetRecentHotGamesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityNetGameHotList>>(await (await _game.PostAsync("/item/query/recent-hot-games", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> SearchItemsByType(string userId, string userToken, int itemType, int offset = 0, int length = 20)
	{
		return SearchItemsByTypeAsync(userId, userToken, itemType, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> SearchItemsByTypeAsync(string userId, string userToken, int itemType, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { itemType, offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/search-by-type", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> GetDefaultRtxMap(string userId, string userToken)
	{
		return GetDefaultRtxMapAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> GetDefaultRtxMapAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/get-default-rtx-map", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> SearchByIid(string userId, string userToken, string iid)
	{
		return SearchByIidAsync(userId, userToken, iid).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> SearchByIidAsync(string userId, string userToken, string iid)
	{
		string body = JsonSerializer.Serialize(new { item_id = iid }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/search-by-iid", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 物品组件 (源码: aeg.cs)

	public Entities<EntityComponentQuery> SearchUserItemsByMcVersion(string userId, string userToken, string mcVersion)
	{
		return SearchUserItemsByMcVersionAsync(userId, userToken, mcVersion).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityComponentQuery>> SearchUserItemsByMcVersionAsync(string userId, string userToken, string mcVersion)
	{
		string body = JsonSerializer.Serialize(new { mc_version = mcVersion }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityComponentQuery>>(await (await _game.PostAsync("/item/search-user-items-by-mcversion", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UserIsPurchaseItem(string userId, string userToken, string itemId)
	{
		return UserIsPurchaseItemAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UserIsPurchaseItemAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item/user-is-purchase-item", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityResourceItem> SearchMcGameItemList(string userId, string userToken, string mcVersion)
	{
		return SearchMcGameItemListAsync(userId, userToken, mcVersion).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityResourceItem>> SearchMcGameItemListAsync(string userId, string userToken, string mcVersion)
	{
		string body = JsonSerializer.Serialize(new { mc_version = mcVersion }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityResourceItem>>(await (await _game.PostAsync("/item/query/search-mcgame-item-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchRequirementsByItems(string userId, string userToken, List<string> itemIds)
	{
		return SearchRequirementsByItemsAsync(userId, userToken, itemIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchRequirementsByItemsAsync(string userId, string userToken, List<string> itemIds)
	{
		string body = JsonSerializer.Serialize(new { item_ids = itemIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item-requirement/query/search-by-items", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchTagsByItem(string userId, string userToken, string itemId)
	{
		return SearchTagsByItemAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchTagsByItemAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item-tag/query/search-by-item", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchTagsByIds(string userId, string userToken, List<string> tagIds)
	{
		return SearchTagsByIdsAsync(userId, userToken, tagIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchTagsByIdsAsync(string userId, string userToken, List<string> tagIds)
	{
		string body = JsonSerializer.Serialize(new { tag_ids = tagIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/tag/query/search-by-ids", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 物品价格 (源码: adk.cs)

	public Entities<EntityItemPrice> SearchPricesByItems(string userId, string userToken, List<string> itemIds)
	{
		return SearchPricesByItemsAsync(userId, userToken, itemIds).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityItemPrice>> SearchPricesByItemsAsync(string userId, string userToken, List<string> itemIds)
	{
		string body = JsonSerializer.Serialize(new { item_ids = itemIds }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityItemPrice>>(await (await _game.PostAsync("/item-price/query/search-by-items", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 物品包 (源码: adl.cs)

	public Entities<EntityPackage> GetItemPackage(string userId, string userToken, string itemId)
	{
		return GetItemPackageAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPackage>> GetItemPackageAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityPackage>>(await (await _game.PostAsync("/item-package", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityPackage> GetMyBuyPackageList(string userId, string userToken)
	{
		return GetMyBuyPackageListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPackage>> GetMyBuyPackageListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityPackage>>(await (await _game.PostAsync("/my-buy-package-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityPackage> GetMyPackageList(string userId, string userToken)
	{
		return GetMyPackageListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPackage>> GetMyPackageListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityPackage>>(await (await _game.PostAsync("/my-package-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityPackageItem> LoadItemPackageItemList(string userId, string userToken, string packageId)
	{
		return LoadItemPackageItemListAsync(userId, userToken, packageId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPackageItem>> LoadItemPackageItemListAsync(string userId, string userToken, string packageId)
	{
		string body = JsonSerializer.Serialize(new { package_id = packageId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityPackageItem>>(await (await _game.PostAsync("/load-item-package-item-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 商城 (源码: aej.cs)

	public Entities<EntityShopItemType> GetShopItemTypes(string userId, string userToken)
	{
		return GetShopItemTypesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityShopItemType>> GetShopItemTypesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityShopItemType>>(await (await _game.PostAsync("/shop-item-type", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityShopItem> GetShopItems(string userId, string userToken, int offset = 0, int length = 20)
	{
		return GetShopItemsAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityShopItem>> GetShopItemsAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityShopItem>>(await (await _game.PostAsync("/shop-item", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 购买列表 (源码: adr.cs)

	public EntityResponse SearchBuyItemList(string userId, string userToken, int offset = 0, int length = 20)
	{
		return SearchBuyItemListAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchBuyItemListAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item-buy-list/query/search-buy-item-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchBuyItemListV2(string userId, string userToken, int offset = 0, int length = 20)
	{
		return SearchBuyItemListV2Async(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchBuyItemListV2Async(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item-buy-list/query/search-buy-item-list-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchMcGameItems(string userId, string userToken, int offset = 0, int length = 20)
	{
		return SearchMcGameItemsAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchMcGameItemsAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/item-buy-list/query/search-mcgame-items", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 表情/外观 (源码: adr.cs)

	public Entities<EntityAppearanceItem> GetEmoteList(string userId, string userToken)
	{
		return GetEmoteListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityAppearanceItem>> GetEmoteListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityAppearanceItem>>(await (await _game.PostAsync("/user-emotes/get-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetUserEmotes(string userId, string userToken)
	{
		return GetUserEmotesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserEmotesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-emotes/get-user-emotes", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetUserEmotes(string userId, string userToken, List<string> emoteIds)
	{
		return SetUserEmotesAsync(userId, userToken, emoteIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetUserEmotesAsync(string userId, string userToken, List<string> emoteIds)
	{
		string body = JsonSerializer.Serialize(new { emote_ids = emoteIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-emotes/set-user-emotes", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region PE物品 (源码: adi.cs)

	public EntityResponse SendPeUserItem(string userId, string userToken, string itemId, string targetUserId)
	{
		return SendPeUserItemAsync(userId, userToken, itemId, targetUserId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SendPeUserItemAsync(string userId, string userToken, string itemId, string targetUserId)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId, target_user_id = targetUserId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/pe-user-item-send", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityPeItemDetail> GetPeItemDetailV2(string userId, string userToken, string itemId)
	{
		return GetPeItemDetailV2Async(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityPeItemDetail>> GetPeItemDetailV2Async(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityPeItemDetail>>(await (await _game.PostAsync("/pe-item-detail-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 购买

	public EntityResponse PurchaseItem(string userId, string userToken, string itemId, int quantity = 1)
	{
		return PurchaseItemAsync(userId, userToken, itemId, quantity).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> PurchaseItemAsync(string userId, string userToken, string itemId, int quantity)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId, quantity }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-item-purchase", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityBuyItemResult> GetBuyItemResult(string userId, string userToken, string orderId)
	{
		return GetBuyItemResultAsync(userId, userToken, orderId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityBuyItemResult>> GetBuyItemResultAsync(string userId, string userToken, string orderId)
	{
		string body = JsonSerializer.Serialize(new { order_id = orderId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityBuyItemResult>>(await (await _game.PostAsync("/buy-item-result", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityItemPromoteInfo> GetPromotionInfo(string userId, string userToken)
	{
		return GetPromotionInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityItemPromoteInfo>> GetPromotionInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entity<EntityItemPromoteInfo>>(await (await _game.PostAsync("/interconn/web/get-promotion-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
