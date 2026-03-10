using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using EastSide.Core.Cipher.Protocol.WPFLauncher.Entity;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.CloudSave;

public class CloudSaveProtocol : WPFLauncherBase
{
	public CloudSaveProtocol(string version) : base(version)
	{
	}

	public Entity<EntityCloudSave> GetCloudSaveV2(string userId, string userToken, string entityId)
	{
		return GetCloudSaveV2Async(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> GetCloudSaveV2Async(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSaveUpload> UploadCloudSaveV2(string userId, string userToken, string entityId, int partnum)
	{
		return UploadCloudSaveV2Async(userId, userToken, entityId, partnum).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSaveUpload>> UploadCloudSaveV2Async(string userId, string userToken, string entityId, int partnum)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId, partnum }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSaveUpload>>(await (await _game.PostAsync("/cloud-save-upload-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSaveDownload> DownloadCloudSaveV2(string userId, string userToken, string entityId, int partnum)
	{
		return DownloadCloudSaveV2Async(userId, userToken, entityId, partnum).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSaveDownload>> DownloadCloudSaveV2Async(string userId, string userToken, string entityId, int partnum)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId, partnum }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSaveDownload>>(await (await _game.PostAsync("/cloud-save-download-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityCloudSave> QueryMySaveList(string userId, string userToken, string itemId, ulong offset = 0, ulong length = 50)
	{
		return QueryMySaveListAsync(userId, userToken, itemId, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityCloudSave>> QueryMySaveListAsync(string userId, string userToken, string itemId, ulong offset, ulong length)
	{
		string body = JsonSerializer.Serialize(new { item_id = itemId, offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2/query/my-save-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSave> RepairCloudSaveV2(string userId, string userToken, string entityId)
	{
		return RepairCloudSaveV2Async(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> RepairCloudSaveV2Async(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2/repair", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSave> MergeCloudSaveV2(string userId, string userToken, string entityId)
	{
		return MergeCloudSaveV2Async(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> MergeCloudSaveV2Async(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2/merge", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSave> GetUserCloudSaveInfoV2(string userId, string userToken, string entityId)
	{
		return GetUserCloudSaveInfoV2Async(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> GetUserCloudSaveInfoV2Async(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/user-cloud-save-info-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSave> DeleteCloudSaveV2(string userId, string userToken, string entityId)
	{
		return DeleteCloudSaveV2Async(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> DeleteCloudSaveV2Async(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new { entity_id = entityId }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2/delete", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityCloudSave> CreateCloudSaveV2(string userId, string userToken, bool isVipOnly)
	{
		return CreateCloudSaveV2Async(userId, userToken, isVipOnly).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCloudSave>> CreateCloudSaveV2Async(string userId, string userToken, bool isVipOnly)
	{
		string body = JsonSerializer.Serialize(new { vip_only = isVipOnly ? "true" : "no" }, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCloudSave>>(await (await _game.PostAsync("/cloud-save-v2", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}
}
