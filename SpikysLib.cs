using SpikysLib.Configs.UI;
using Terraria.ModLoader;

namespace SpikysLib;


public class SpikysLib : Mod {

    public override void Load() {
		MonoModHooks.Modify(Reflection.ConfigElement.DrawSelf, Text.ILTextColors);
		Currencies.GetCurrencies();
	}

    public override void Unload() {
		Currencies.ClearCurrencies();
	}

}
