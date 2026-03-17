using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Game;

public class GameProtocol : WPFLauncherBase
{
	public GameProtocol(string version) : base(version)
	{
	}

	#region 游戏角色 (源码: aes.cs)

	public Entities<EntityGameCharacter> GetUserGameCharacters(string userId, string userToken, int gameType, string gameId, int offset = 0, int length = 10)
	{
		return GetUserGameCharactersAsync(userId, userToken, gameType, gameId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGameCharacter>> GetUserGameCharactersAsync(string userId, string userToken, int gameType, string gameId, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length, user_id = userId, game_id = gameId, game_type = gameType }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/query/user-game-characters", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> CreateCharacter(string userId, string userToken, int gameType, string gameId, string name)
	{
		return CreateCharacterAsync(userId, userToken, gameType, gameId, name).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> CreateCharacterAsync(string userId, string userToken, int gameType, string gameId, string name)
	{
		string body = JsonSerializer.Serialize(new { entity_id = "", game_id = gameId, game_type = gameType, user_id = userId, name, create_time = 555555 }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> DeleteCharacter(string userId, string userToken, string entityId)
	{
		return DeleteCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> DeleteCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> PreDeleteCharacter(string userId, string userToken, string entityId)
	{
		return PreDeleteCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> PreDeleteCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/pre-delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityGameCharacter> CancelPreDeleteCharacter(string userId, string userToken, string entityId)
	{
		return CancelPreDeleteCharacterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityGameCharacter>> CancelPreDeleteCharacterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/cancel-pre-delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 收藏和装扮

	public EntityResponse AddFavoriteItem(string userId, string userToken, string itemId)
	{
		return AddFavoriteItemAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> AddFavoriteItemAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { iid = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-favorite-item/add", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DeleteFavoriteItem(string userId, string userToken, string itemId)
	{
		return DeleteFavoriteItemAsync(userId, userToken, itemId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DeleteFavoriteItemAsync(string userId, string userToken, string itemId)
	{
		string body = JsonSerializer.Serialize(new { iid = itemId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-favorite-item/delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetUserOutfit(string userId, string userToken)
	{
		return GetUserOutfitAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetUserOutfitAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-outfit/get", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UpdateUserOutfit(string userId, string userToken, string outfitData)
	{
		return UpdateUserOutfitAsync(userId, userToken, outfitData).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UpdateUserOutfitAsync(string userId, string userToken, string outfitData)
	{
		string body = JsonSerializer.Serialize(new { outfitData }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-outfit/update", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetEmotesList(string userId, string userToken)
	{
		return GetEmotesListAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetEmotesListAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-emotes/get-list", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
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
		string body = JsonSerializer.Serialize(new { emoteIds }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-emotes/set-user-emotes", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
