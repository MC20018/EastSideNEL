namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Types;

public enum VisibilityStatus
{
	PUBLIC = 0,
	FRIEND = 1,
	PRIVATE = 2,
	PASSWORD = 3
}

public enum RoomVisibleStatus
{
	OPEN = 0,
	FRIEND = 1,
	HIDDEN = 2
}

public enum MealStatus
{
	OFF = 0,
	ON = 1
}

public enum JobType
{
	none,
	cleanup,
	sync_tier
}
