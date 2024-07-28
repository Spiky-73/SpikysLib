using Terraria.ModLoader;
using SpikysLib.Configs.UI;
using SpikysLib.UI;
using SpikysLib.Configs;

namespace SpikysLib;

public class SpikysLib : Mod {

    public override void Load() {
		TextElement.Load();
		ConfigHelper.Load();
		CursorLoader.Load();
		PlayerHelper.Load();
	}

    public override void Unload() {
		TextElement.Unload();
		ConfigHelper.Unload();
		CursorLoader.Unload();
		PlayerHelper.Unload();
	}
}
