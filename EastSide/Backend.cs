using System;
using System.IO;
using System.Threading.Tasks;
using Codexus.Cipher.Connection.Protocols;
using Codexus.Cipher.Protocol;
using Codexus.Development.SDK.Manager;
using Codexus.Game.Launcher.Utils;
using Codexus.Interceptors;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Codexus.OpenSDK.Yggdrasil;
using EastSide.Core.Network;
using EastSide.Manager;
using EastSide.Type;
using EastSide.Utils;
using Serilog;
using FileUtil = Codexus.Game.Launcher.Utils.FileUtil;

namespace EastSide;

public static class Backend
{
    private static readonly TaskCompletionSource _initialized = new();

    public static Task WaitForInitAsync() => _initialized.Task;

    public static void Initialize()
    {
        AuthManager.Instance.LoadFromDisk();
        LocalHttpServer.Instance.Start();
        _ = InitializeServicesAsync();
    }

    private static async Task InitializeServicesAsync()
    {
        try
        {
            await Task.Run(async () =>
            {
                FileUtil.CreateDirectorySafe(PathUtil.CustomModsPath);
                AppState.Services = await CreateServicesAsync();
                InternalQuery.Initialize();
                await InitializeSystemComponentsAsync();
                Notification.Send("EastSide","Welcome!");
            });
            _initialized.TrySetResult();
            Log.Information("后端服务初始化完成");
        }
        catch (Exception ex)
        {
            _initialized.TrySetResult();
            Log.Information($"服务初始化失败: {ex.Message}");
        }
    }

    private static async Task<Services> CreateServicesAsync()
    {
        var launcherVersion = await WPFLauncher.GetLatestVersionAsync();
        var yggdrasil = new StandardYggdrasil(new YggdrasilData
        {
            LauncherVersion = launcherVersion,
            Channel = "netease",
            CrcSalt = "B73962A7833192F9CAD0D68A2AA4462E"
        });
        return new Services(yggdrasil);
    }

    private static async Task InitializeSystemComponentsAsync()
    {
        var pluginDir = EastSide.Utils.FileUtil.GetPluginDirectory();
        Directory.CreateDirectory(pluginDir);
        UserManager.Instance.ReadUsersFromDisk();
        Interceptor.EnsureLoaded();
        PacketManager.Instance.RegisterPacketFromAssembly(typeof(Backend).Assembly);
        PacketManager.Instance.EnsureRegistered();
        HttpUrlRewriter.Initialize();
        try
        {
            PluginManager.Instance.EnsureUninstall();
            PluginManager.Instance.LoadPlugins(pluginDir);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "插件加载失败");
        }
        await Task.CompletedTask;
    }

    public static async Task<(bool Success, string Message)> RandomLogin4399Async()
    {
        try
        {
            using var registerTool = new EastSide.Core.Utils.Channel4399Register();
            var account = await registerTool.RegisterAsync(
                inputCaptchaAsync: async (url) => string.Empty,
                idCardFunc: () => new Codexus.Development.SDK.Entities.IdCard
                {
                    Name = EastSide.Core.Utils.Channel4399Register.GenerateChineseName(),
                    IdNumber = EastSide.Core.Utils.Channel4399Register.GenerateRandomIdCard()
                }
            );

            Log.Information("4399账号注册成功: {Account}", account.Account);

            var loginHandler = new EastSide.Handlers.PC.Login.Login4399();
            var result = await Task.Run(() => loginHandler.Execute(account.Account, account.Password));

            if (result is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var tv = item?.GetType().GetProperty("type")?.GetValue(item)?.ToString();
                    if (tv == "Success_login")
                    {
                        var eid = item?.GetType().GetProperty("entityId")?.GetValue(item)?.ToString();
                        await Manager.UserManager.Instance.SaveUsersToDiskAsync();
                        return (true, $"登录成功！EntityId: {eid}");
                    }
                }
            }

            return (false, "登录失败");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "随机登录失败");
            return (false, ex.Message);
        }
    }

    private static async Task<(bool Success, string Message, string? Extra)> ParseLoginResultAsync(object? result)
    {
        if (result == null) return (false, "登录失败", null);

        var resultType = result.GetType();
        var typeProp = resultType.GetProperty("type");
        if (typeProp != null)
        {
            var typeValue = typeProp.GetValue(result)?.ToString() ?? "";

            if (typeValue.EndsWith("_error", StringComparison.OrdinalIgnoreCase))
            {
                var msg = resultType.GetProperty("message")?.GetValue(result)?.ToString() ?? "登录失败";
                return (false, msg, null);
            }

            if (typeValue.StartsWith("captcha_required"))
            {
                var captchaUrl = resultType.GetProperty("captchaUrl")?.GetValue(result)?.ToString();
                return (false, "需要验证码", captchaUrl);
            }

            if (typeValue == "login_x19_verify")
            {
                var verifyUrl = resultType.GetProperty("verify_url")?.GetValue(result)?.ToString();
                return (false, "需要安全验证，请在浏览器中完成", verifyUrl);
            }
        }

        if (result is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                var tv = item?.GetType().GetProperty("type")?.GetValue(item)?.ToString();
                if (tv == "Success_login")
                {
                    var eid = item?.GetType().GetProperty("entityId")?.GetValue(item)?.ToString();
                    await Manager.UserManager.Instance.SaveUsersToDiskAsync();
                    return (true, $"登录成功！EntityId: {eid}", null);
                }
            }
        }

        return (false, "登录失败", null);
    }

    public static async Task<(bool Success, string Message)> Login4399Async(string account, string password)
    {
        try
        {
            var handler = new Handlers.PC.Login.Login4399();
            var result = await Task.Run(() => handler.Execute(account, password));
            var parsed = await ParseLoginResultAsync(result);
            return (parsed.Success, parsed.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "4399登录失败");
            return (false, ex.Message);
        }
    }

    public static async Task<(bool Success, string Message)> LoginNeteaseAsync(string email, string password)
    {
        try
        {
            var handler = new Handlers.PC.Login.LoginX19();
            var result = await Task.Run(() => handler.Execute(email, password));
            var parsed = await ParseLoginResultAsync(result);
            return (parsed.Success, parsed.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "网易邮箱登录失败");
            return (false, ex.Message);
        }
    }

    public static async Task<(bool Success, string Message)> LoginCookieAsync(string cookie)
    {
        try
        {
            var handler = new Handlers.PC.Login.LoginCookie();
            var result = await Task.Run(() => handler.Execute(cookie));
            var parsed = await ParseLoginResultAsync(result);
            return (parsed.Success, parsed.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Cookie登录失败");
            return (false, ex.Message);
        }
    }
}
