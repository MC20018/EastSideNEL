namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Types;

public enum ClientStatus
{
	None = -1,
	SAVE_UPLOADING = 0,
	SAVE_COMPRESSING = 1,
	SAVE_UNCOMPRESSING = 2,
	SAVE_RESTORING = 3,
	SETTING_COMPONENTS = 4
}

public enum CloseChangeType
{
	MinimizeToSystemTray = 0,

	ExitProgram = 1
}

public enum GlobalConfigType
{
	MAIL_TIMELINE = 1,
	CHAT_VERSION_ID = 2,
	GROUP_VERSION_ID = 3,
	START_GAME_GUIDE_ID = 4,
	MAIL_MAX_MAILID = 36865
}
