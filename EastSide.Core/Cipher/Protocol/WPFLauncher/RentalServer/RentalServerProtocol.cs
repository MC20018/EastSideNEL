using System.Text.Encodings.Web;
using System.Text.Json;
using Codexus.Cipher.Entities;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.RentalServer;

public class RentalServerProtocol : WPFLauncherBase
{
	public RentalServerProtocol(string version) : base(version)
	{
	}

	#region 服务器信息 (源码: aer.cs)

	public Entity<RentalServerDetailEntity> GetRentalServerDetails(string userId, string userToken, string entityId)
	{
		return GetRentalServerDetailsAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerDetailEntity>> GetRentalServerDetailsAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerDetailEntity>>(await (await _rental.PostAsync("/rental-server-details/get", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<MyRentalServerEntity> GetMyRentalServer(string userId, string userToken, string entityId)
	{
		return GetMyRentalServerAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<MyRentalServerEntity>> GetMyRentalServerAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<MyRentalServerEntity>>(await (await _rental.PostAsync("/my-rental-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<MyRentalServerInfoEntity> GetMyRentalServerInfo(string userId, string userToken, string entityId)
	{
		return GetMyRentalServerInfoAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<MyRentalServerInfoEntity>> GetMyRentalServerInfoAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<MyRentalServerInfoEntity>>(await (await _rental.PostAsync("/my-rental-server-info", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<MyRentalServerJobEntity> GetMyRentalServerJob(string userId, string userToken, string entityId)
	{
		return GetMyRentalServerJobAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<MyRentalServerJobEntity>> GetMyRentalServerJobAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<MyRentalServerJobEntity>>(await (await _rental.PostAsync("/my-rental-server-job", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerPlayInfoEntity> GetRentalServerPlayInfo(string userId, string userToken, string entityId)
	{
		return GetRentalServerPlayInfoAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerPlayInfoEntity>> GetRentalServerPlayInfoAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerPlayInfoEntity>>(await (await _rental.PostAsync("/rental-server-play-info", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 服务器控制 (源码: aer.cs)

	public Entity<RentalServerControlEntity> ControlRentalServer(string userId, string userToken, string serverId, int status)
	{
		return ControlRentalServerAsync(userId, userToken, serverId, status).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerControlEntity>> ControlRentalServerAsync(string userId, string userToken, string serverId, int status)
	{
		string body = JsonSerializer.Serialize(new RentalServerControlRequest { ServerId = serverId, Status = status }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerControlEntity>>(await (await _rental.PostAsync("/rental-server-control", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerControlEntity> GetRentalServerStatus(string userId, string userToken, string entityId)
	{
		return GetRentalServerStatusAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerControlEntity>> GetRentalServerStatusAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerControlEntity>>(await (await _rental.PostAsync("/rental-server-control/get-status", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 玩家管理 (源码: aer.cs)

	public Entities<RentalServerPlayerEntity> GetRentalServerPlayers(string userId, string userToken,
		string serverId, int playerStatus = 0, int offset = 0, int length = 50, bool isOnline = false)
	{
		return GetRentalServerPlayersAsync(userId, userToken, serverId, playerStatus, offset, length, isOnline).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerPlayerEntity>> GetRentalServerPlayersAsync(string userId, string userToken,
		string serverId, int playerStatus, int offset, int length, bool isOnline)
	{
		string body = JsonSerializer.Serialize(new SearchByServerRequest
		{
			ServerId = serverId, Status = playerStatus, Offset = offset, Length = length, IsOnline = isOnline
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<RentalServerPlayerEntity>>(await (await _rental.PostAsync("/rental-server-player/query/search-by-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerPlayerLoginEntity> PlayerLoginEnter(string userId, string userToken, string entityId)
	{
		return PlayerLoginEnterAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerPlayerLoginEntity>> PlayerLoginEnterAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerPlayerLoginEntity>>(await (await _rental.PostAsync("/rental-server-player-login/enter", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 世界管理 (源码: aer.cs)

	public Entity<RentalServerWorldEnterEntity> EnterRentalServerWorld(string userId, string userToken, string serverId, string worldId)
	{
		return EnterRentalServerWorldAsync(userId, userToken, serverId, worldId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerWorldEnterEntity>> EnterRentalServerWorldAsync(string userId, string userToken, string serverId, string worldId)
	{
		string body = JsonSerializer.Serialize(new { a = serverId, b = worldId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerWorldEnterEntity>>(await (await _rental.PostAsync("/rental-server-world-enter", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerWorldSettingsEntity> GetRentalServerWorldSettings(string userId, string userToken, string entityId)
	{
		return GetRentalServerWorldSettingsAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerWorldSettingsEntity>> GetRentalServerWorldSettingsAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerWorldSettingsEntity>>(await (await _rental.PostAsync("/rental-server-world-settings", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 套餐 (源码: aer.cs)

	public Entities<RentalServerMealEntity> GetRentalServerMeals(string userId, string userToken, string serverType = "docker")
	{
		return GetRentalServerMealsAsync(userId, userToken, serverType).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerMealEntity>> GetRentalServerMealsAsync(string userId, string userToken, string serverType)
	{
		string body = JsonSerializer.Serialize(new { server_type = serverType }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<RentalServerMealEntity>>(await (await _rental.PostAsync("/rental-server-meal/query/available/by-sort-type", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerMealChangeEntity> ChangeRentalServerMeal(string userId, string userToken, string serverId, string mealId)
	{
		return ChangeRentalServerMealAsync(userId, userToken, serverId, mealId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerMealChangeEntity>> ChangeRentalServerMealAsync(string userId, string userToken, string serverId, string mealId)
	{
		string body = JsonSerializer.Serialize(new { server_id = serverId, meal_id = mealId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerMealChangeEntity>>(await (await _rental.PostAsync("/rental-server-meal-change", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerKeepEntity> KeepRentalServer(string userId, string userToken,
		string entityId, string mealId, uint capacity, ulong activeTime, ulong expireTime)
	{
		return KeepRentalServerAsync(userId, userToken, entityId, mealId, capacity, activeTime, expireTime).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerKeepEntity>> KeepRentalServerAsync(string userId, string userToken,
		string entityId, string mealId, uint capacity, ulong activeTime, ulong expireTime)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId, meal_id = mealId, capacity, active_time = activeTime, expire_time = expireTime }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerKeepEntity>>(await (await _rental.PostAsync("/rental-server-keep", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 备份管理 (源码: aer.cs)

	public Entities<RentalServerBackupEntity> GetRentalServerBackups(string userId, string userToken, string worldId)
	{
		return GetRentalServerBackupsAsync(userId, userToken, worldId).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerBackupEntity>> GetRentalServerBackupsAsync(string userId, string userToken, string worldId)
	{
		string body = JsonSerializer.Serialize(new { world_id = worldId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<RentalServerBackupEntity>>(await (await _rental.PostAsync("/rental-server-backup/query/search-by-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerDownloadEntity> GetRentalServerBackupDownloadUrl(string userId, string userToken, string worldId, int backupId)
	{
		return GetRentalServerBackupDownloadUrlAsync(userId, userToken, worldId, backupId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerDownloadEntity>> GetRentalServerBackupDownloadUrlAsync(string userId, string userToken, string worldId, int backupId)
	{
		string body = JsonSerializer.Serialize(new { world_id = worldId, backup_id = backupId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerDownloadEntity>>(await (await _rental.PostAsync("/rental-server-backup-download-url", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerBackupEntity> UploadRentalServerBackup(string userId, string userToken, string worldId, int backupId, string backupName)
	{
		return UploadRentalServerBackupAsync(userId, userToken, worldId, backupId, backupName).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerBackupEntity>> UploadRentalServerBackupAsync(string userId, string userToken, string worldId, int backupId, string backupName)
	{
		string body = JsonSerializer.Serialize(new { world_id = worldId, backup_id = backupId, backup_name = backupName }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerBackupEntity>>(await (await _rental.PostAsync("/rental-server-backup-upload", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerUploadUrlEntity> GetRentalServerBackupUploadUrl(string userId, string userToken, string worldId)
	{
		return GetRentalServerBackupUploadUrlAsync(userId, userToken, worldId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerUploadUrlEntity>> GetRentalServerBackupUploadUrlAsync(string userId, string userToken, string worldId)
	{
		string body = JsonSerializer.Serialize(new { world_id = worldId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerUploadUrlEntity>>(await (await _rental.PostAsync("/rental-server-backup-upload-url", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerBackupRestoreEntity> RestoreRentalServerBackup(string userId, string userToken, string worldId, uint backupId)
	{
		return RestoreRentalServerBackupAsync(userId, userToken, worldId, backupId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerBackupRestoreEntity>> RestoreRentalServerBackupAsync(string userId, string userToken, string worldId, uint backupId)
	{
		string body = JsonSerializer.Serialize(new { world_id = worldId, backup_id = backupId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerBackupRestoreEntity>>(await (await _rental.PostAsync("/rental-server-backup-restore", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 服务器配置 (源码: aer.cs)

	public Entities<RentalServerSupportedVersionsEntity> GetSupportedVersions(string userId, string userToken)
	{
		return GetSupportedVersionsAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerSupportedVersionsEntity>> GetSupportedVersionsAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<RentalServerSupportedVersionsEntity>>(await (await _rental.PostAsync("/rental-server-supported-versions", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<RentalServerTemplateEntity> GetRentalServerTemplates(string userId, string userToken)
	{
		return GetRentalServerTemplatesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerTemplateEntity>> GetRentalServerTemplatesAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<Entities<RentalServerTemplateEntity>>(await (await _rental.PostAsync("/rental-server-tpl", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<RentalServerImageUrlEntity> GetRentalServerImageUrl(string userId, string userToken, string entityId)
	{
		return GetRentalServerImageUrlAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerImageUrlEntity>> GetRentalServerImageUrlAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<RentalServerImageUrlEntity>>(await (await _rental.PostAsync("/rental-server-image-url", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<RentalServerUnstablePluginEntity> GetUnstablePlugin(string userId, string userToken, string entityId)
	{
		return GetUnstablePluginAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entities<RentalServerUnstablePluginEntity>> GetUnstablePluginAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<RentalServerUnstablePluginEntity>>(await (await _rental.PostAsync("/rental-server-unstable-plugin", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<RentalServerMCAdminTokenEntity> GetMyRentalServerAdminSession(string userId, string userToken, string entityId)
	{
		return GetMyRentalServerAdminSessionAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<RentalServerMCAdminTokenEntity>> GetMyRentalServerAdminSessionAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<RentalServerMCAdminTokenEntity>>(await (await _rental.PostAsync("/my-rental-server-admin-session", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 服务器查询 (源码: aer.cs)

	public Entities<Entity.RentalServerEntity> QueryAvailablePublicServer(string userId, string userToken, int offset = 0, int length = 20)
	{
		return QueryAvailablePublicServerAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<Entity.RentalServerEntity>> QueryAvailablePublicServerAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<Entity.RentalServerEntity>>(await (await _rental.PostAsync("/rental-server/query/available-public-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<Entity.RentalServerEntity> QueryAvailableFriendServer(string userId, string userToken, int offset = 0, int length = 20)
	{
		return QueryAvailableFriendServerAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<Entity.RentalServerEntity>> QueryAvailableFriendServerAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<Entity.RentalServerEntity>>(await (await _rental.PostAsync("/rental-server/query/available-friend-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<Entity.RentalServerEntity> QueryRecommendServer(string userId, string userToken, int offset = 0, int length = 20)
	{
		return QueryRecommendServerAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<Entity.RentalServerEntity>> QueryRecommendServerAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<Entity.RentalServerEntity>>(await (await _rental.PostAsync("/rental-server/query/recommend-server", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<Entity.RentalServerEntity> SearchByUserServer(string userId, string userToken, string serverId)
	{
		return SearchByUserServerAsync(userId, userToken, serverId).GetAwaiter().GetResult();
	}

	private async Task<Entities<Entity.RentalServerEntity>> SearchByUserServerAsync(string userId, string userToken, string serverId)
	{
		string body = JsonSerializer.Serialize(new { server_id = serverId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<Entity.RentalServerEntity>>(await (await _rental.PostAsync("/rental-server/query/search-by-user", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 初始化 (源码: aer.cs)

	public EntityResponse InitializeServerByTemplate(string userId, string userToken, string serverId, string tplId)
	{
		return InitializeServerByTemplateAsync(userId, userToken, serverId, tplId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> InitializeServerByTemplateAsync(string userId, string userToken, string serverId, string tplId)
	{
		string body = JsonSerializer.Serialize(new { server_id = serverId, tpl_id = tplId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _rental.PostAsync("/my-rental-server/initialize_by_tpl", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SyncWorldTier(string userId, string userToken, string serverId)
	{
		return SyncWorldTierAsync(userId, userToken, serverId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SyncWorldTierAsync(string userId, string userToken, string serverId)
	{
		string body = JsonSerializer.Serialize(new { server_id = serverId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _rental.PostAsync("/my-rental-server/sync_world_tier", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
