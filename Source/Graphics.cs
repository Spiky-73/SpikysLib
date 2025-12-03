using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace SpikysLib;

[Obsolete($"use {nameof(GraphicsHelper)} instead", true)] // v1.3
public static class Graphics {
    [Obsolete($"use {nameof(GraphicsHelper)}.{nameof(GraphicsHelper.DrawTileFrame)} instead", true)] // v1.3
    public static void DrawTileFrame(this SpriteBatch spriteBatch, int tile, Vector2 position, Vector2 origin, float scale) => GraphicsHelper.DrawTileFrame(spriteBatch, tile, position, origin, scale);
    [Obsolete($"use {nameof(GraphicsHelper)}.{nameof(GraphicsHelper.DrawStringWithShadow)} instead", true)] // v1.3
    public static void DrawStringWithShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) => GraphicsHelper.DrawStringWithShadow(spriteBatch, font, text, position, color, rotation, origin, scale, spread);
    [Obsolete($"use {nameof(GraphicsHelper)}.{nameof(GraphicsHelper.DrawStringShadow)} instead", true)] // v1.3
    public static void DrawStringShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) => GraphicsHelper.DrawStringShadow(spriteBatch, font, text, position, color, rotation, origin, scale, spread);
}