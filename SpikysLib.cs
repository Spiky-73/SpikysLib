using Terraria.ModLoader;
using SpikysLib.Configs.UI;
using SpikysLib.UI;
using SpikysLib.Configs;
using SpikysLib.Localization;
using System;

namespace SpikysLib;

public class SpikysLib : Mod {

    public override void Load() {
		TextElement.Load();
		ConfigHelper.Load();
		CursorLoader.Load();
		PlayerHelper.Load();
		LanguageHelper.Load();
		MonoModHooks.Add(Reflection.Mod.AutoloadConfig, HookPreLoadMod);
	}

    public override void Unload() {
		TextElement.Unload();
		ConfigHelper.Unload();
		CursorLoader.Unload();
		PlayerHelper.Unload();
		LanguageHelper.Unload();
	}

	public static void HookPreLoadMod(Action<Mod> orig, Mod mod) {
		if (mod is IPreLoadMod preLoadMod) preLoadMod.PreLoadMod();
		orig(mod);
	}
}
