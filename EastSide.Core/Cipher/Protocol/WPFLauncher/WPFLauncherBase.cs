using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using Codexus.Cipher.Utils.Http;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher;

public abstract class WPFLauncherBase
{
	protected readonly HttpWrapper _client;
	protected readonly HttpWrapper _core;
	protected readonly HttpWrapper _game;
	protected readonly HttpWrapper _gateway;
	protected readonly HttpWrapper _rental;
	protected HttpWrapper _transfer;
	protected readonly string _version;

	protected static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	protected WPFLauncherBase(string gameVersion, string transferServerUrl = null)
	{
		_version = gameVersion;
		string userAgent = "WPFLauncher/" + gameVersion;

		_client = new HttpWrapper("https://x19mclobt.nie.netease.com", builder =>
		{
			builder.UserAgent(userAgent);
		});

		_core = new HttpWrapper("https://x19obtcore.nie.netease.com:8443", builder =>
		{
			builder.UserAgent(userAgent);
		});

		_game = new HttpWrapper("https://x19apigatewayobt.nie.netease.com", builder =>
		{
			builder.UserAgent(userAgent);
		});

		_gateway = new HttpWrapper("https://x19apigatewayobt.nie.netease.com", builder =>
		{
			builder.UserAgent(userAgent);
		});

		_rental = new HttpWrapper("https://x19mclobt.nie.netease.com", builder =>
		{
			builder.UserAgent(userAgent);
		});

		if (!string.IsNullOrEmpty(transferServerUrl))
		{
			_transfer = new HttpWrapper(transferServerUrl, builder =>
			{
				builder.UserAgent(userAgent);
			});
		}
	}

	public void SetTransferServer(string transferServerUrl)
	{
		_transfer?.Dispose();
		_transfer = new HttpWrapper(transferServerUrl, builder =>
		{
			builder.UserAgent("WPFLauncher/" + _version);
		});
	}

	protected WPFLauncherBase(HttpWrapper client, HttpWrapper core, HttpWrapper game, HttpWrapper gateway, HttpWrapper rental, HttpWrapper transfer = null)
	{
		_client = client;
		_core = core;
		_game = game;
		_gateway = gateway;
		_rental = rental;
		_transfer = transfer;
	}

	public virtual void Dispose()
	{
		_client?.Dispose();
		_core?.Dispose();
		_game?.Dispose();
		_gateway?.Dispose();
		_rental?.Dispose();
		_transfer?.Dispose();
	}
}
