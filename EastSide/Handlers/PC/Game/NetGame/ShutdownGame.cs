using EastSide.Manager;

namespace EastSide.Handlers.Game.NetServer;

public class ShutdownGame
{
    public object[] Execute(IEnumerable<string> identifiers)
    {
        var closed = new List<string>();
        foreach (var s in identifiers)
        {
            if (string.IsNullOrWhiteSpace(s)) continue;
            if (Guid.TryParse(s, out var id))
            {
                GameManager.Instance.ShutdownInterceptor(id);
                closed.Add(s);
            }
        }
        var payloads = new object[]
        {
            new { type = "shutdown_ack", identifiers = closed.ToArray() },
            new { type = "channels_updated" }
        };
        return payloads;
    }
}
