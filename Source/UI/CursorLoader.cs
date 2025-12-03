using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace SpikysLib.UI;

public static class CursorLoader {

    public const int VanillaCount = 20;

    public static ReadOnlyCollection<ModCursor> Cursors => s_modCursors.AsReadOnly();

    public static ModCursor RegisterCursor(Mod mod, Asset<Texture2D> cursor) {
        ModCursor mc = new(mod, cursor, VanillaCount + s_modCursors.Count);
        s_modCursors.Add(mc);
        return mc;
    }

    internal static void Load() {
        On_Main.DrawInterface_36_Cursor += HookDrawCustomCursor;
    }
    internal static void Unload() {
        s_modCursors.Clear();
    }
    
    private static void HookDrawCustomCursor(On_Main.orig_DrawInterface_36_Cursor orig) {
        if (VanillaCount <= Main.cursorOverride && Main.cursorOverride < VanillaCount + s_modCursors.Count) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw(s_modCursors[Main.cursorOverride-VanillaCount].Cursor.Value, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0, default, Main.cursorScale, 0, 0f);
        } else orig();
    }

    private static readonly List<ModCursor> s_modCursors = [];
}
