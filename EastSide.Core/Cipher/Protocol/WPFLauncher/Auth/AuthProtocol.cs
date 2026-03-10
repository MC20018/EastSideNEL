using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using EastSide.Core.Entity.WFPLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Auth;

public class AuthProtocol : WPFLauncherBase
{
	public AuthProtocol(string version) : base(version)
	{
	}

	#region 认证

	public EntityResponse LoginOtp(string userId, string userToken, string otpCode)
	{
		return LoginOtpAsync(userId, userToken, otpCode).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> LoginOtpAsync(string userId, string userToken, string otpCode)
	{
		string body = JsonSerializer.Serialize(new { otpCode }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _core.PostAsync("/login-otp", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse AuthenticationOtp(string userId, string userToken, string otpCode)
	{
		return AuthenticationOtpAsync(userId, userToken, otpCode).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> AuthenticationOtpAsync(string userId, string userToken, string otpCode)
	{
		string body = JsonSerializer.Serialize(new { otpCode }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _core.PostAsync("/authentication-otp", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse Authentication(string userId, string userToken)
	{
		return AuthenticationAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> AuthenticationAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _core.PostAsync("/authentication", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse UserRealnameAuth(string userId, string userToken, string realName, string idCard)
	{
		return UserRealnameAuthAsync(userId, userToken, realName, idCard).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> UserRealnameAuthAsync(string userId, string userToken, string realName, string idCard)
	{
		string body = JsonSerializer.Serialize(new { realName, idCard }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/user-realname-auth", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion

	#region 配置

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

	public EntityResponse GetClientConfig(string userId, string userToken)
	{
		return GetClientConfigAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetClientConfigAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _game.PostAsync("/interconn/web/client-config", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse CheckClientLibraryChange(string userId, string userToken, string currentVersion)
	{
		return CheckClientLibraryChangeAsync(userId, userToken, currentVersion).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> CheckClientLibraryChangeAsync(string userId, string userToken, string currentVersion)
	{
		string body = JsonSerializer.Serialize(new { currentVersion }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/client-library-info/check-change", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SearchClientLibraryByMcVersion(string userId, string userToken, string mcVersion)
	{
		return SearchClientLibraryByMcVersionAsync(userId, userToken, mcVersion).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SearchClientLibraryByMcVersionAsync(string userId, string userToken, string mcVersion)
	{
		string body = JsonSerializer.Serialize(new { mcVersion }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/client-library-info/search-by-mc-version", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetGameClientInfo(string userId, string userToken)
	{
		return GetGameClientInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetGameClientInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/game-client-info/get", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetCppGameClientInfo(string userId, string userToken)
	{
		return GetCppGameClientInfoAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetCppGameClientInfoAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/cpp-game-client-info", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetGamePatchInfo(string userId, string userToken, string patchId)
	{
		return GetGamePatchInfoAsync(userId, userToken, patchId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetGamePatchInfoAsync(string userId, string userToken, string patchId)
	{
		string body = JsonSerializer.Serialize(new { patchId }, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/game-patch-info/", body, delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetGameUpdaterV2(string userId, string userToken)
	{
		return GetGameUpdaterV2Async(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetGameUpdaterV2Async(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/game-updater-v2", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetMcVersion(string userId, string userToken)
	{
		return GetMcVersionAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetMcVersionAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _client.PostAsync("/mc-version/get", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse GetServerTime(string userId, string userToken)
	{
		return GetServerTimeAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> GetServerTimeAsync(string userId, string userToken)
	{
		return JsonSerializer.Deserialize<EntityResponse>(await (await _core.PostAsync("/server-time", "", delegate(HttpWrapper.HttpWrapperBuilder builder)
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	#endregion
}
