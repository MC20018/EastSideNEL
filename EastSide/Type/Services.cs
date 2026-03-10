using Codexus.OpenSDK.Yggdrasil;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Serilog;

namespace EastSide.Type;

internal class Services(
    StandardYggdrasil Yggdrasil
    )
{ 
    public StandardYggdrasil Yggdrasil { get; private set; } = Yggdrasil;
    
    public string CurrentSalt { get; private set; } = string.Empty;
    
    public string LauncherVersion { get; private set; } = string.Empty;
    
    public void UpdateSalt(string newSalt, string launcherVersion)
    {
        Log.Information("[CRC] UpdateSalt调用: newSalt={NewSalt}, CurrentSalt={CurrentSalt}", newSalt, CurrentSalt);
        
        if (string.IsNullOrEmpty(newSalt))
        {
            Log.Warning("[CRC] 尝试更新空盐，忽略");
            return;
        }
        
        if (CurrentSalt == newSalt)
        {
            Log.Information("[CRC] 盐未变化，跳过重新初始化");
            return;
        }
        
        var oldSalt = CurrentSalt;
        CurrentSalt = newSalt;
        LauncherVersion = launcherVersion;
        
        Yggdrasil = new StandardYggdrasil(new YggdrasilData
        {
            LauncherVersion = launcherVersion,
            Channel = "netease",
            CrcSalt = newSalt
        });
        
        Log.Information("[CRC] Yggdrasil已重新初始化: {OldSalt} -> {NewSalt}", oldSalt, newSalt);
    }
}
