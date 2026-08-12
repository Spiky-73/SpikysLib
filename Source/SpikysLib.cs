using Terraria.ModLoader;
using SpikysLib.Configs.UI;
using SpikysLib.UI;
using SpikysLib.Configs;
using SpikysLib.Localization;
using System;
using Terraria.ModLoader.Config.UI;
using MonoMod.Cil;
using SpikysLib.IL;
using Terraria.ModLoader.Config;
using System.Collections;

namespace SpikysLib;

public class SpikysLib : Mod {

    public override void Load() {
        TextElement.Load();
        ConfigHelper.Load();
        CursorLoader.Load();
        PlayerHelper.Load();
        LanguageHelper.Load();
        MonoModHooks.Add(TypeHelper.GetMethod((Mod m) => m.AutoloadConfig), HookPreLoadMod);
        MonoModHooks.Modify(TypeHelper.GetMethod(() => UIModConfig.WrapIt), ILWrapObject);
    }

    public override void Unload() {
        TextElement.Unload();
        ConfigHelper.Unload();
        CursorLoader.Unload();
        PlayerHelper.Unload();
        LanguageHelper.Unload();
    }

    private static void HookPreLoadMod(Action<Mod> orig, Mod mod) {
        if (mod is IPreLoadMod preLoadMod) preLoadMod.PreLoadMod();
        orig(mod);
    }

    private static void ILWrapObject(ILContext il) {
        ILCursor cursor = new(il);
        cursor.GotoNextLoc(MoveType.After, out var customUI, i => i.Previous.MatchCall(() => ConfigManager.GetCustomAttributeFromMemberThenMemberType<CustomModConfigItemAttribute>), 1);
        cursor.EmitLdloc(customUI).EmitLdarg(6);
        cursor.EmitDelegate((CustomModConfigItemAttribute? customUI, Type? arrayType) => {
            if (SpikysLibConfig.Instance.disableFixCustomModConfigItemList || arrayType is null) return customUI;
            // If we want the custom UI for a list item, use the type only as the one of the member is for the list itself
            return (CustomModConfigItemAttribute?)Attribute.GetCustomAttribute(arrayType, typeof(CustomModConfigItemAttribute), inherit: true);
        });
        cursor.EmitStloc(customUI);
    }
}
