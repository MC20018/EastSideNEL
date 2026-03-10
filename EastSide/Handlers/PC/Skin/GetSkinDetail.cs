using System;
using System.Linq;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Skin;
using EastSide.Manager;
using EastSide.Type;
using Serilog;

namespace EastSide.Handlers.PC.Skin;

public class GetSkinDetailResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool NotLogin { get; set; }
    public SkinItemData? Item { get; set; }
}

public class GetSkinDetail
{
    public GetSkinDetailResult Execute(string accountId, string itemId)
    {
        var user = UserManager.Instance.GetAvailableUser(accountId);
        if (user == null) return new GetSkinDetailResult { NotLogin = true };

        try
        {
            var skinList = new Entities<EntitySkin>
            {
                Data = new[] { new EntitySkin { EntityId = itemId } }
            };
            var details = AppState.X19.GetSkinDetails(user.UserId, user.AccessToken, skinList);
            var skin = details.Data?.FirstOrDefault();
            if (skin == null)
                return new GetSkinDetailResult { Success = false, Message = "未找到皮肤" };

            return new GetSkinDetailResult
            {
                Success = true,
                Item = new SkinItemData
                {
                    EntityId = skin.EntityId,
                    Name = skin.Name,
                    PreviewUrl = skin.TitleImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取皮肤详情失败");
            return new GetSkinDetailResult { Success = false, Message = "获取详情失败" };
        }
    }
}
