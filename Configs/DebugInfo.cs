using Terraria.ModLoader.Config;

namespace SpikysLib.Configs;

public sealed class DebugInfo : ModConfig {
    public bool displayGuids;

    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static DebugInfo Instance = null!;
}