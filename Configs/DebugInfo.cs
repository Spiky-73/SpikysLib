using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace SpikysLib.Configs;

public sealed class DebugInfo : ModConfig {
    public bool displayGuids;

    [DefaultValue(""), JsonProperty] internal string lastPlayedVersion = "";
    
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static DebugInfo Instance = null!;
}