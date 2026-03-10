namespace EastSide.Core.Cipher.Protocol.WPFLauncher.Types;

public static class ErrorCode
{
	public const int UNKNOWN = 105;
	public const int SUCCESS = 200;
	public const int CREATED = 201;
	public const int UNAUTHORIZED = 401;
	public const int GONE = 410;
	public const int LOCKED = 420;
	public const int UNPROCESSABLE = 422;
	public const int SERVER_ERROR = 500;
	public const int SERVICE_UNAVAILABLE = 503;
	public const int CUSTOM_701 = 701;
	public const int CUSTOM_702 = 702;
	public const int CUSTOM_703 = 703;
	public const int CUSTOM_801 = 801;
	public const int CUSTOM_901 = 901;
}

public enum UnisdkOrderStatus
{
	Status0 = 0,
	Status1 = 1,
	Status2 = 2,
	Status3 = 3,
	Status4 = 4,
	Status5 = 5,
	Status6 = 6,
	Status10 = 10,
	Status11 = 11,
	Status12 = 12,
	Status13 = 13
}

public enum UnisdkPayStatus
{
	Init = 0,
	Paying = 1,
	Success = 2,
	Failed = 3
}

public enum UnisdkCallbackStatus
{
	Init = 0,
	Success = 2,
	Failed = 3
}

public enum UnisdkVerifyStatus
{
	Init = 0,
	Verified = 1,
	Unverified = 2,
	Error = 3
}
