
using EastSide.Handlers.PC.Login;
using EastSide.Manager;

namespace EastSide.Handlers.PC.Account
{
    public class DeleteAccount
    {
        public object Execute(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                return new { type = "delete_error", message = "entityId为空" };
            }
            UserManager.Instance.RemoveAvailableUser(entityId);
            UserManager.Instance.RemoveUser(entityId);
            var items = GetAccount.GetAccountItems();
            return new { type = "accounts", items };
        }
    }
}
