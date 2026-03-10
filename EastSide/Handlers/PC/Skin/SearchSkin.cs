using System;
using System.Linq;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Skin;
using EastSide.Manager;
using EastSide.Type;
using Serilog;

namespace EastSide.Handlers.PC.Skin;

public class SearchSkin
{
    public GetFreeSkinResult Execute(string keyword)
    {
        var last = UserManager.Instance.GetLastAvailableUser();
        if (last == null) return new GetFreeSkinResult { NotLogin = true };

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return new GetFreeSkinResult { Success = false, Message = "请输入搜索关键词" };
        }

        try
        {
            Codexus.Cipher.Entities.Entities<EntitySkin> list;
            try
            {
                list = AppState.X19.QueryFreeSkinByName(last.UserId, last.AccessToken, keyword);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "搜索皮肤失败: {Message}", ex.Message);
                return new GetFreeSkinResult { Success = false, Message = ex.Message};
            }

            var baseData = list.Data;
            var baseCount = baseData.Length;

            if (baseCount == 0)
            {
                return new GetFreeSkinResult { Success = true, Items = new(), HasMore = false };
            }

            Codexus.Cipher.Entities.Entities<EntitySkin>? detailed = null;
            try
            {
                detailed = AppState.X19.GetSkinDetails(last.UserId, last.AccessToken, list);
            }
            catch (Exception ex)
            {
                if (SettingManager.Instance.Get().Debug) Log.Error(ex, "皮肤详情获取失败，退回基础列表");
            }

            var data = detailed?.Data ?? baseData;
            Log.Information("搜索皮肤详情数量={Count}", data.Length);

            var items = data.Select(s => new SkinItemData
            {
                EntityId = s.EntityId,
                Name = s.Name,
                PreviewUrl = s.TitleImageUrl
            }).ToList();

            return new GetFreeSkinResult { Success = true, Items = items, HasMore = false };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "搜索皮肤失败");
            return new GetFreeSkinResult { Success = false, Message = "搜索失败" };
        }
    }
}
