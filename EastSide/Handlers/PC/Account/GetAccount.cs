using System.Collections.Generic;
using System.Linq;
using EastSide.Manager;

namespace EastSide.Handlers.PC.Account;

public class AccountItem
{
    public string EntityId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = "offline";
    public string Alias { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class GetAccount
{
    public static List<AccountItem> GetAccountList()
    {
        var users = UserManager.Instance.GetUsersNoDetails();
        return users.Select(u => new AccountItem
        {
            EntityId = u.UserId,
            Channel = u.Channel,
            Status = u.Authorized ? "online" : "offline",
            Alias = u.Alias,
            Type = u.Type
        }).ToList();
    }

    public static object[] GetAccountItems()
    {
        var users = UserManager.Instance.GetUsersNoDetails();
        return users.Select(u => new { entityId = u.UserId, channel = u.Channel, status = u.Authorized ? "online" : "offline", alias = u.Alias}).ToArray();
    }

    public static bool HasAuthorizedUser()
    {
        var users = UserManager.Instance.GetUsersNoDetails();
        return users.Any(u => u.Authorized);
    }
}
