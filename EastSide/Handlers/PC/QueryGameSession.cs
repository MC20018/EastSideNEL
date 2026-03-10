using EastSide.Entities.Web.NEL;
using EastSide.Manager;

namespace EastSide.Handlers.PC;

public class QueryGameSession
{
    public object Execute()
    {
        var list = new List<EntityQueryGameSessions>();

        foreach (var interceptor in GameManager.Instance.GetQueryInterceptors())
        {
            list.Add(new EntityQueryGameSessions
            {
                Id = "interceptor-" + interceptor.Id,
                ServerName = interceptor.Server,
                Guid = interceptor.Name.ToString(),
                CharacterName = interceptor.Role,
                ServerVersion = interceptor.Version,
                StatusText = "Running",
                ProgressValue = 0,
                Type = "Interceptor",
                LocalAddress = interceptor.LocalAddress
            });
        }

        foreach (var launcher in GameManager.Instance.GetQueryLaunchers())
        {
            list.Add(new EntityQueryGameSessions
            {
                Id = "launcher-" + launcher.Id,
                ServerName = launcher.Server,
                Guid = launcher.Name.ToString(),
                CharacterName = launcher.Role,
                ServerVersion = launcher.Version,
                StatusText = launcher.StatusMessage,
                ProgressValue = launcher.Progress,
                Type = "Launcher",
                LocalAddress = $"{launcher.Server}:{launcher.ProcessId}"
            });
        }

        return new { type = "query_game_session", items = list };
    }
}
