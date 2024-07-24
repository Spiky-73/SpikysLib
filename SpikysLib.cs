using Terraria.ModLoader;
using SpikysLib.Extensions;
using SpikysLib.Configs.UI;
using SpikysLib.UI;

namespace SpikysLib;

public class SpikysLib : Mod {

    public override void Load() {
		TextElement.Load();
		PortConfig.Load();
		CursorLoader.Load();
		Currencies.Load();
		PlayerExtensions.Load();
	}

    public override void Unload() {
		TextElement.Unload();
		PortConfig.Unload();
		CursorLoader.Unload();
		Currencies.Unload();
		PlayerExtensions.Unload();
	}
}
