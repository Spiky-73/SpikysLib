using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace SpikysLib.UI;

public record class ModCursor {
    internal ModCursor(Mod mod, Asset<Texture2D> cursor, int type) {
        Mod = mod;
        Cursor = cursor;
        Type = type;
    }

    public bool IsCurrent => Main.cursorOverride == Type;
    public void SetAsCurrent() => Main.cursorOverride = Type;

    public Mod Mod { get; }
    public Asset<Texture2D> Cursor { get; }
    public int Type { get; }
}