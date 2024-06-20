using SpikysLib.Configs.UI;
using SpikysLib.Extensions;
using Terraria.ModLoader;

namespace SpikysLib;

// TODO custom float element (no slider -> text)
public class SpikysLib : Mod {

    public override void Load() {
		MonoModHooks.Modify(Reflection.ConfigElement.DrawSelf, TextElement.ILTextColors);
		MonoModHooks.Add(Reflection.ConfigManager.Load, ModConfigExtensions.HookPort);
		Currencies.GetCurrencies();
	}

    public override void Unload() {
		Currencies.ClearCurrencies();
	}

}
