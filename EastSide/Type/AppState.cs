namespace EastSide.Type;

using Codexus.Cipher.Protocol;

public class AppState
{
    private static WPFLauncher? _x19;

    public static WPFLauncher X19
    {
        get
        {
            if (_x19 == null)
            {
                _x19 = new WPFLauncher();
            }
            return _x19;
        }
    }

    public static void ResetX19()
    {
        _x19?.Dispose();
        _x19 = new WPFLauncher();
    }

    public static readonly Com4399 Com4399 = new Com4399();
    public const string DataFolder = "data";
    internal static Services? Services;
    public const string AppVersion = "ES_new 1.0";
}
