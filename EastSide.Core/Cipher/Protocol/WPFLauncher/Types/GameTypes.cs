using System;

namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Types;

public enum GStatus
{
	NONE = -1,
	NORMAL = 1,
	NEED_DOWNLOAD = 2,
	DOWNLOADING = 3,
	END_DOWNLOAD = 4,
	LAUNCHING = 5,
	PLAYING = 6,
	CLOUD_DOWNLOADING = 7,
	CLOUD_UPLOADING = 8,
	UPDATING = 9
}

public enum GType
{
	NONE = -1,
	SINGLE_GAME = 1,
	NET_GAME = 2,
	MC_GAME = 7,
	SERVER_GAME = 8,
	LAN_GAME = 9,
	ONLINE_LOBBY_GAME = 10
}

public enum GameMasterTypeId
{
	JavaGame = 0,
	CppGame = 1
}

[Serializable]
public enum GameVersion : uint
{
	NONE = 0u,
	V_CPP = 100000000u,
	V_X64_CPP = 300000000u,
	V_RTX = 200000000u,
	V_1_6_4 = 1006004u,
	V_1_7_2 = 1007002u,
	V_1_7_10 = 1007010u,
	V_1_8 = 1008000u,
	V_1_8_8 = 1008008u,
	V_1_8_9 = 1008009u,
	V_1_9_4 = 1009004u,
	V_1_10_2 = 1010002u,
	V_1_11_2 = 1011002u,
	V_1_12 = 1012000u,
	V_1_12_2 = 1012002u,
	V_1_13_2 = 1013002u,
	V_1_14_3 = 1014003u,
	V_1_15 = 1015000u,
	V_1_16 = 1016000u,
	V_1_18 = 1018000u,
	V_1_19_2 = 1019002u,
	V_1_20 = 1020000u,
	V_1_20_6 = 1020006u,
	V_1_21 = 1021000u
}

public enum TestResult
{
	Init = -1,
	Ok = 0,
	TimeOut = 1,
	Failed = 2,
	LauncherError = 3,
	GameError = 4
}

public enum AppearanceRarityLevel
{
	Normal = 0,
	Excellent = 10,
	Rare = 20,
	Epic = 30,
	Legend = 40,
	SpecialLegend = 50,
	Fairy = 60
}
