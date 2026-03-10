namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Types;

public static class ProtocolMessageType
{
	public const int MSG_INTERNAL_NEG19 = -19;
	public const int MSG_INTERNAL_NEG18 = -18;
	public const int MSG_INTERNAL_NEG17 = -17;
	public const int MSG_INTERNAL_NEG16 = -16;
	public const int MSG_INTERNAL_NEG15 = -15;
	public const int MSG_INTERNAL_NEG14 = -14;
	public const int MSG_INTERNAL_NEG13 = -13;
	public const int MSG_INTERNAL_NEG12 = -12;
	public const int MSG_INTERNAL_NEG11 = -11;
	public const int MSG_INTERNAL_NEG10 = -10;
	public const int MSG_INTERNAL_NEG6 = -6;
	public const int MSG_INTERNAL_NEG5 = -5;
	public const int MSG_INTERNAL_NEG4 = -4;
	public const int MSG_INTERNAL_NEG3 = -3;
	public const int MSG_INTERNAL_NEG2 = -2;
	public const int MSG_INTERNAL_NEG1 = -1;
	public const int MSG_CYCLIC = 0;
	public const int MSG_LOGIN = 1;
	public const int MSG_LOGOUT = 2;
	public const int MSG_HEARTBEAT = 3;
	public const int MSG_KICK = 4;
	public const int MSG_RECONNECT = 5;
	public const int MSG_SYNC = 6;
	public const int MSG_ACK = 7;
	public const int MSG_NOTIFY = 8;
	public const int MSG_ERROR = 9;
	public const int MSG_CHAT = 10;
	public const int MSG_CHAT_READ = 11;
	public const int MSG_CHAT_DELETE = 12;
	public const int MSG_CHAT_RECALL = 13;
	public const int MSG_CHAT_TYPING = 14;
	public const int MSG_FRIEND_REQUEST = 17;
	public const int MSG_FRIEND_ACCEPT = 18;
	public const int MSG_GROUP_MSG = 20;
	public const int MSG_GROUP_JOIN = 21;
	public const int MSG_GROUP_LEAVE = 22;
	public const int MSG_GROUP_KICK = 23;
	public const int MSG_GROUP_INVITE = 24;
	public const int MSG_GROUP_INVITE_REPLY = 25;
	public const int MSG_GROUP_RENAME = 29;
	public const int MSG_GROUP_IMG = 31;
	public const int MSG_GROUP_PERM = 32;
	public const int MSG_ROOM_UPDATE = 46;
	public const int MSG_ROOM_JOIN = 50;
	public const int MSG_ROOM_LEAVE = 51;
	public const int MSG_ROOM_KICK = 53;
	public const int MSG_ROOM_CHAT = 56;
	public const int MSG_ROOM_READY = 64;
	public const int MSG_ROOM_START = 67;
	public const int MSG_ROOM_END = 75;
	public const int MSG_MAIL = 100;
	public const int MSG_SYSTEM_NOTIFY = 200;
	public const int MSG_SYSTEM_BROADCAST = 201;
	public const int MSG_FRIEND_ONLINE = 300;
	public const int MSG_FRIEND_OFFLINE = 301;
	public const int MSG_FRIEND_STATUS = 302;
	public const int MSG_FRIEND_DELETE = 303;
	public const int MSG_FRIEND_BLOCK = 304;
	public const int MSG_FRIEND_UNBLOCK = 305;
	public const int MSG_FRIEND_MARK = 306;
	public const int MSG_FRIEND_APPLY = 307;
	public const int MSG_FRIEND_REPLY = 308;
	public const int MSG_FRIEND_SEARCH = 309;
	public const int MSG_FRIEND_LIST = 310;
	public const int MSG_FRIEND_INFO = 311;
	public const int MSG_FRIEND_DETAIL = 312;
	public const int MSG_FRIEND_ONLINE_LIST = 313;
	public const int MSG_FRIEND_ADDME = 314;
	public const int MSG_FRIEND_CHANGE_MARK = 315;
	public const int MSG_FRIEND_SEARCH_RESULT = 316;
	public const int MSG_SERVER_STATUS = 802;
	public const int MSG_INTERNAL_NEG300 = -300;
	public const int MSG_LOBBY_STATUS = 400;
	public const int MSG_LOBBY_JOIN = 401;
	public const int MSG_LOBBY_LEAVE = 402;
	public const int MSG_LOBBY_KICK = 403;
	public const int MSG_GAME_STATUS = 502;
	public const int MSG_PAYMENT = 2100;
	public const int MSG_CC_JOIN = 27000;
	public const int MSG_CC_LEAVE = 27001;
	public const int MSG_CC_AUDIO = 14004;
	public const int MSG_CC_TALK = 27003;
	public const int MSG_CC_SEARCH = 27004;
	public const int MSG_CC_CONFIG = 11305;
	public const int MSG_LOBBY_GAME_ENTER = 20000;
	public const int MSG_LOBBY_GAME_CONTROL = 20001;
	public const int MSG_LOBBY_ROOM = 20002;
	public const int MSG_LOBBY_ROOM_ENTER = 20003;
	public const int MSG_LOBBY_MEMBER = 20004;
	public const int MSG_LOBBY_MEMBER_KICK = 20005;
	public const int MSG_LOBBY_MEMBER_TICK = 20006;
	public const int MSG_LOBBY_BACKUP = 20007;
	public const int MSG_LOBBY_COLLECTION = 20008;
	public const int MSG_LOBBY_GAME = 20009;
	public const int MSG_LOBBY_GAME_LIST = 20141;
}

public static class GamePipelineState
{
	public const int MAX = int.MaxValue;
	public const int NEAR_MAX = 1879048191;
	public const int IDLE = 0;
	public const int INIT = 10;
	public const int INIT_DONE = 11;
	public const int PREPARE = 20;
	public const int PREPARE_CHECK = 21;
	public const int PREPARE_DONE = 22;
	public const int DOWNLOAD = 50;
	public const int DOWNLOAD_DONE = 51;
	public const int INSTALL = 100;
	public const int INSTALL_STEP1 = 101;
	public const int INSTALL_STEP2 = 102;
	public const int INSTALL_STEP3 = 103;
	public const int INSTALL_STEP4 = 104;
	public const int INSTALL_STEP5 = 105;
	public const int INSTALL_STEP6 = 106;
	public const int INSTALL_STEP7 = 107;
	public const int INSTALL_STEP8 = 108;
	public const int INSTALL_STEP9 = 109;
	public const int INSTALL_STEP10 = 110;
	public const int INSTALL_STEP11 = 111;
	public const int INSTALL_STEP12 = 112;
	public const int INSTALL_STEP13 = 113;
	public const int LAUNCH = 200;
	public const int LAUNCH_STEP1 = 201;
	public const int LAUNCH_STEP2 = 202;
	public const int LAUNCH_STEP3 = 203;
	public const int LAUNCH_STEP4 = 204;
	public const int LAUNCH_STEP5 = 205;
	public const int COMPLETE = 300;
}

public static class TaskState
{
	public const int Idle = 0;
	public const int Running = 1;
	public const int Paused = 2;
	public const int Cancelled = 3;
	public const int Completed = 4;
	public const int Failed = 5;
	public const int Retrying = 6;
	public const int Timeout = 7;
}

public enum FeatureFlags
{
	None = 65536,
	Flag0 = 0,
	Flag1 = 1,
	Flag2 = 2,
	Flag3 = 4,
	Flag4 = 8,
	Flag5 = 16,
	Flag6 = 32,
	Flag7 = 64,
	Flag8 = 128,
	Flag9 = 256,
	Flag10 = 512,
	Flag11 = 1024,
	Flag12 = 2048,
	Flag13 = 4096,
	Flag14 = 8192
}

public static class ResourceConstants
{
	public const int RETRY_COUNT = 5;
	public const int TIMEOUT_SHORT = 10000;
	public const int TIMEOUT_MEDIUM = 70000;
	public const int TIMEOUT_DEFAULT = 10000;
	public const int TIMEOUT_LONG = 250000;
	public const int TIMEOUT_EXTRA_LONG = 960000;
}

public static class HeartbeatConstants
{
	public const int INTERVAL = 300000;
}
