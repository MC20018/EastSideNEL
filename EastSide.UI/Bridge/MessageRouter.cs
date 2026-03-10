using System;
using System.Text.Json;
using System.Threading.Tasks;
using Photino.NET;
using Serilog;

namespace EastSide.UI.Bridge;

public static class MessageRouter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static void HandleMessage(object? sender, string message)
    {
        _ = HandleMessageAsync(sender as PhotinoWindow, message);
    }

    private static async Task HandleMessageAsync(PhotinoWindow? window, string rawMessage)
    {
        if (window == null) return;

        try
        {
            var request = JsonSerializer.Deserialize<BridgeRequest>(rawMessage, JsonOptions);
            if (request == null)
            {
                Log.Warning("收到无效消息: {Message}", rawMessage);
                return;
            }

            Log.Debug("收到前端消息: action={Action}, requestId={RequestId}", request.Action, request.RequestId);

            var response = request.Action switch
            {
                "auth:getStatus" => await AuthHandler.GetStatus(request),
                "auth:login" => await AuthHandler.Login(request),
                "auth:logout" => AuthHandler.Logout(request),
                "auth:register:sendCode" => await AuthHandler.SendRegisterCode(request),
                "auth:register:verifyCode" => await AuthHandler.VerifyCode(request),
                "auth:register:complete" => await AuthHandler.CompleteRegister(request),
                "auth:activateCard" => await AuthHandler.ActivateCard(request),

                "settings:get" => SettingsHandler.GetSettings(request),
                "settings:update" => SettingsHandler.UpdateSettings(request),

                "system:announcement" => await SystemHandler.GetAnnouncement(request),

                "overview:recent" => OverviewHandler.GetRecent(request),

                "account:list" => await AccountHandler.GetAccounts(request),
                "account:activate" => await AccountHandler.Activate(request),
                "account:logout" => AccountHandler.Logout(request),
                "account:delete" => await AccountHandler.Delete(request),
                "account:updateAlias" => await AccountHandler.UpdateAlias(request),
                "account:randomLogin" => await AccountHandler.RandomLogin(request),
                "account:loginAdd" => await AccountHandler.LoginAdd(request),

                "network:list" => await NetworkHandler.ListServers(request),
                "network:detail" => await NetworkHandler.GetDetail(request),
                "network:roles" => await NetworkHandler.GetRoles(request),
                "network:createRole" => await NetworkHandler.CreateRole(request),
                "network:deleteRole" => await NetworkHandler.DeleteRole(request),
                "network:accounts" => await NetworkHandler.GetAccounts(request),
                "network:selectAccount" => await NetworkHandler.SelectAccount(request),
                "network:join" => await NetworkHandler.JoinServer(request),

                "rental:list" => await RentalHandler.ListServers(request),
                "rental:detail" => await RentalHandler.GetDetail(request),
                "rental:roles" => await RentalHandler.GetRoles(request),
                "rental:createRole" => await RentalHandler.CreateRole(request),
                "rental:deleteRole" => await RentalHandler.DeleteRole(request),
                "rental:accounts" => await NetworkHandler.GetAccounts(request),
                "rental:selectAccount" => await NetworkHandler.SelectAccount(request),
                "rental:join" => await RentalHandler.JoinServer(request),

                "skin:list" => await SkinHandler.ListSkins(request),
                "skin:search" => await SkinHandler.SearchSkins(request),
                "skin:detail" => await SkinHandler.GetDetail(request),
                "skin:purchase" => await SkinHandler.Purchase(request),
                "skin:buyResult" => await SkinHandler.BuyResult(request),
                "skin:apply" => await SkinHandler.ApplySkin(request),
                "skin:accounts" => await NetworkHandler.GetAccounts(request),

                "session:list" => await SessionHandler.ListSessions(request),
                "session:shutdown" => await SessionHandler.Shutdown(request),

                "plugin:available" => await PluginHandler.ListAvailable(request),
                "plugin:installed" => await PluginHandler.ListInstalled(request),
                "plugin:install" => await PluginHandler.Install(request),
                "plugin:uninstall" => await PluginHandler.Uninstall(request),

                "system:restart" => SystemHandler.Restart(request),
                "system:about" => SystemHandler.GetAbout(request),
                "system:openUrl" => SystemHandler.OpenUrl(request),
                "system:setBackdrop" => SystemHandler.SetBackdrop(request),
                "system:browseFile" => SystemHandler.BrowseFile(request),

                "log:info" => LogHandler.Info(request),
                "log:error" => LogHandler.Error(request),

                "window:drag" => WindowHandler.StartDrag(request),
                "window:minimize" => WindowHandler.Minimize(request),
                "window:maximize" => WindowHandler.Maximize(request),
                "window:close" => WindowHandler.Close(request),

                _ => BridgeResponse.Fail(request, $"未知 action: {request.Action}")
            };

            var json = JsonSerializer.Serialize(response, JsonOptions);
            window.SendWebMessage(json);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "处理前端消息异常: {Message}", rawMessage);
            try
            {
                var error = new BridgeResponse
                {
                    Action = "error",
                    RequestId = "",
                    Success = false,
                    Data = JsonSerializer.SerializeToElement(new { message = ex.Message }, JsonOptions)
                };
                window.SendWebMessage(JsonSerializer.Serialize(error, JsonOptions));
            }
            catch {  }
        }
    }
}

public class BridgeRequest
{
    public string Action { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public JsonElement? Data { get; set; }
}

public class BridgeResponse
{
    public string Action { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public JsonElement? Data { get; set; }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static BridgeResponse Ok(BridgeRequest req, object? data = null) => new()
    {
        Action = req.Action,
        RequestId = req.RequestId,
        Success = true,
        Data = data != null ? JsonSerializer.SerializeToElement(data, JsonOpts) : null
    };

    public static BridgeResponse Fail(BridgeRequest req, string message) => new()
    {
        Action = req.Action,
        RequestId = req.RequestId,
        Success = false,
        Data = JsonSerializer.SerializeToElement(new { message }, JsonOpts)
    };
}
