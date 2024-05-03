using SpikysLib.Configs.UI;
using Terraria.ModLoader;

namespace SpikysLib;

// TODO custom float element (no slider -> text)
public class SpikysLib : Mod {

    public override void Load() {
		MonoModHooks.Modify(Reflection.ConfigElement.DrawSelf, TextElement.ILTextColors);
		Currencies.GetCurrencies();
	}

    public override void Unload() {
		Currencies.ClearCurrencies();
	}

}
