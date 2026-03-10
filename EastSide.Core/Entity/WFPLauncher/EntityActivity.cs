using System.Text.Json.Serialization;

namespace EastSide.Core.Entity.WFPLauncher;

public class EntityActivityStateFlag
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("state")] public int State { get; set; }
	[JsonPropertyName("flag")] public string Flag { get; set; } = string.Empty;
}

public class EntityActivityDate
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("start_time")] public long StartTime { get; set; }
	[JsonPropertyName("end_time")] public long EndTime { get; set; }
}

public class EntityActivityInfo
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("activity_type")] public string ActivityType { get; set; } = string.Empty;
	[JsonPropertyName("activity_name")] public string ActivityName { get; set; } = string.Empty;
	[JsonPropertyName("start_time")] public long StartTime { get; set; }
	[JsonPropertyName("end_time")] public long EndTime { get; set; }
}

public class EntityCheckActivity
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("has_reward")] public bool HasReward { get; set; }
	[JsonPropertyName("status")] public int Status { get; set; }
}

public class EntityChargeActivityInfo
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("quest_list")] public EntityChargeActivityQuestItem[] QuestList { get; set; } = System.Array.Empty<EntityChargeActivityQuestItem>();
	[JsonPropertyName("reward_list")] public EntityChargeActivityRewardItem[] RewardList { get; set; } = System.Array.Empty<EntityChargeActivityRewardItem>();
}

public class EntityChargeActivityQuestItem
{
	[JsonPropertyName("quest_id")] public string QuestId { get; set; } = string.Empty;
	[JsonPropertyName("progress")] public int Progress { get; set; }
	[JsonPropertyName("total")] public int Total { get; set; }
	[JsonPropertyName("completed")] public bool Completed { get; set; }
}

public class EntityChargeActivityRewardItem
{
	[JsonPropertyName("reward_id")] public string RewardId { get; set; } = string.Empty;
	[JsonPropertyName("claimed")] public bool Claimed { get; set; }
	[JsonPropertyName("sub_items")] public EntityChargeActivityRewardSubItem[] SubItems { get; set; } = System.Array.Empty<EntityChargeActivityRewardSubItem>();
}

public class EntityChargeActivityRewardSubItem
{
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("count")] public int Count { get; set; }
}

public class EntityChargeActivityReward
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("success")] public bool Success { get; set; }
}

public class EntityNextActivityInfo
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("next_reward_time")] public long NextRewardTime { get; set; }
	[JsonPropertyName("has_next")] public bool HasNext { get; set; }
}

public class EntityLibaoItemInfo
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
	[JsonPropertyName("item_name")] public string ItemName { get; set; } = string.Empty;
	[JsonPropertyName("item_icon")] public string ItemIcon { get; set; } = string.Empty;
	[JsonPropertyName("price")] public int Price { get; set; }
}

public class EntityDiscountPackInfo
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("pack_id")] public string PackId { get; set; } = string.Empty;
	[JsonPropertyName("pack_name")] public string PackName { get; set; } = string.Empty;
	[JsonPropertyName("discount")] public int Discount { get; set; }
	[JsonPropertyName("end_time")] public long EndTime { get; set; }
}

public class EntityDiscountPackPush
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("show")] public bool Show { get; set; }
	[JsonPropertyName("pack_id")] public string PackId { get; set; } = string.Empty;
}

public class EntitySignIn
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("day")] public int Day { get; set; }
	[JsonPropertyName("signed")] public bool Signed { get; set; }
	[JsonPropertyName("reward_id")] public string RewardId { get; set; } = string.Empty;
}

public class EntitySignReward
{
	[JsonPropertyName("entity_id")] public string EntityId { get; set; } = string.Empty;
	[JsonPropertyName("reward_id")] public string RewardId { get; set; } = string.Empty;
	[JsonPropertyName("reward_name")] public string RewardName { get; set; } = string.Empty;
	[JsonPropertyName("reward_count")] public int RewardCount { get; set; }
}
