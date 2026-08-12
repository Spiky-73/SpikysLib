using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace SpikysLib.Configs;

public sealed class SpikysLibConfig : ModConfig {
    public bool disableFixCustomModConfigItemList;
    public bool displayGuids;

    [DefaultValue(""), JsonProperty] internal string lastPlayedVersion = "";
    
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static SpikysLibConfig Instance = null!;
}