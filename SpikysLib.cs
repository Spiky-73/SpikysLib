using SpikysLib.Configs.UI;
using Terraria.ModLoader;

namespace SpikysLib;

public class SpikysLib : Mod {

    public override void Load() {
		MonoModHooks.Modify(Reflection.ConfigElement.DrawSelf, TextElement.ILTextColors);
		MonoModHooks.Add(Reflection.ConfigManager.Load, PortConfig.HookPort);
	}

    public override void Unload() {}

}
