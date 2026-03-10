using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Mail;

public class MailProtocol : WPFLauncherBase
{
	public MailProtocol(string version) : base(version)
	{
	}

	public EntityResponse GetGeneralMailList(string userId, string userToken, int offset = 0, int length = 20)
	{
		return GetGeneralMailListAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetGeneralMailListAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-general-mail-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetStateMailList(string userId, string userToken, int mailState, int offset = 0, int length = 20)
	{
		return GetStateMailListAsync(userId, userToken, mailState, offset, length).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetStateMailListAsync(string userId, string userToken, int mailState, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new { mailState, offset, length }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-state-mail-list", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetDetailMail(string userId, string userToken, string mailId)
	{
		return GetDetailMailAsync(userId, userToken, mailId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetDetailMailAsync(string userId, string userToken, string mailId)
	{
		string body = JsonSerializer.Serialize(new { mailId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-detail-mail", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetMailReward(string userId, string userToken, string mailId)
	{
		return GetMailRewardAsync(userId, userToken, mailId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetMailRewardAsync(string userId, string userToken, string mailId)
	{
		string body = JsonSerializer.Serialize(new { mailId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-mail-reward", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetAllMailReward(string userId, string userToken)
	{
		return GetAllMailRewardAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetAllMailRewardAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/get-all-mail-reward", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse DeleteMail(string userId, string userToken, string mailId)
	{
		return DeleteMailAsync(userId, userToken, mailId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> DeleteMailAsync(string userId, string userToken, string mailId)
	{
		string body = JsonSerializer.Serialize(new { mailId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/delete-mail", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

}
