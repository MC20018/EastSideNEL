using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Home;

public class HomeProtocol : WPFLauncherBase
{
	public HomeProtocol(string version) : base(version)
	{
	}

	#region 宠物管理

	public Entities<EntityPet> GetAllPets(string userId, string userToken)
	{
		return GetAllPetsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPet>> GetAllPetsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityPet>>(await (await _game.PostAsync("/home-get-all-pets", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityPetDetail> GetUserPetDetail(string userId, string userToken, int petNum)
	{
		return GetUserPetDetailAsync(userId, userToken, petNum).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityPetDetail>> GetUserPetDetailAsync(string userId, string userToken, int petNum)
	{
		string body = JsonSerializer.Serialize(new { pet_num = petNum }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityPetDetail>>(await (await _game.PostAsync("/home-get-user-pet-detail", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetPetSkinUnlocked(string userId, string userToken, List<int> skinIds)
	{
		return GetPetSkinUnlockedAsync(userId, userToken, skinIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetPetSkinUnlockedAsync(string userId, string userToken, List<int> skinIds)
	{
		string body = JsonSerializer.Serialize(new { skin_ids = skinIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/home-get-pet-skin-unlocked", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetPetActionUnlocked(string userId, string userToken, List<int> actionIds)
	{
		return GetPetActionUnlockedAsync(userId, userToken, actionIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetPetActionUnlockedAsync(string userId, string userToken, List<int> actionIds)
	{
		string body = JsonSerializer.Serialize(new { action_ids = actionIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/home-get-pet-action-unlocked", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetPetSkin(string userId, string userToken, int petId, string skinName)
	{
		return SetPetSkinAsync(userId, userToken, petId, skinName).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetPetSkinAsync(string userId, string userToken, int petId, string skinName)
	{
		string body = JsonSerializer.Serialize(new { pet_id = petId, name = skinName }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/home-set-pet-skin", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetPetActions(string userId, string userToken, int petId, List<int> actionIds)
	{
		return SetPetActionsAsync(userId, userToken, petId, actionIds).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetPetActionsAsync(string userId, string userToken, int petId, List<int> actionIds)
	{
		string body = JsonSerializer.Serialize(new { pet_id = petId, action_ids = actionIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/home-set-pet-actions", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse RenamePet(string userId, string userToken, int petId, string newName)
	{
		return RenamePetAsync(userId, userToken, petId, newName).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> RenamePetAsync(string userId, string userToken, int petId, string newName)
	{
		string body = JsonSerializer.Serialize(new { pet_id = petId, name = newName }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/home-rename-pet", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityPetSkinCategory> GetPetSkinCategories(string userId, string userToken)
	{
		return GetPetSkinCategoriesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityPetSkinCategory>> GetPetSkinCategoriesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityPetSkinCategory>>(await (await _game.PostAsync("/home-pet-skin-categories", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 家园商店

	public Entities<EntityHomeShopItem> GetHomeShopAll(string userId, string userToken)
	{
		return GetHomeShopAllAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityHomeShopItem>> GetHomeShopAllAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<EntityHomeShopItem>>(await (await _game.PostAsync("/home-shop-all", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityHomeShopItemDetail> GetHomeShopItemDetail(string userId, string userToken, int itemNum)
	{
		return GetHomeShopItemDetailAsync(userId, userToken, itemNum).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityHomeShopItemDetail>> GetHomeShopItemDetailAsync(string userId, string userToken, int itemNum)
	{
		string body = JsonSerializer.Serialize(new { num = itemNum }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityHomeShopItemDetail>>(await (await _game.PostAsync("/home-shop-item-detail", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityHomeShopBuyResult> BuyHomeShopItem(string userId, string userToken, int itemNum, string couponId = "")
	{
		return BuyHomeShopItemAsync(userId, userToken, itemNum, couponId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityHomeShopBuyResult>> BuyHomeShopItemAsync(string userId, string userToken, int itemNum, string couponId)
	{
		object requestBody;
		if (string.IsNullOrEmpty(couponId))
			requestBody = new { item_num = itemNum };
		else
			requestBody = new { item_num = itemNum, coupon_ids = new List<string> { couponId } };

		string body = JsonSerializer.Serialize(requestBody, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityHomeShopBuyResult>>(await (await _game.PostAsync("/home-shop-buy-item", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityHomeItemDetail> GetHomeItemDetail(string userId, string userToken, int itemNum)
	{
		return GetHomeItemDetailAsync(userId, userToken, itemNum).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityHomeItemDetail>> GetHomeItemDetailAsync(string userId, string userToken, int itemNum)
	{
		string body = JsonSerializer.Serialize(new { item_num = itemNum }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityHomeItemDetail>>(await (await _game.PostAsync("/home-item-get-detail", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
