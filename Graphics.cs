using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace SpikysLib;

[Obsolete($"use {nameof(GraphicsHelper)} instead", true)]
public static class Graphics {

    public static void DrawTileFrame(this SpriteBatch spriteBatch, int tile, Vector2 position, Vector2 origin, float scale) => GraphicsHelper.DrawTileFrame(spriteBatch, tile, position, origin, scale);

    public static void DrawStringWithShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) {
        spriteBatch.DrawStringShadow(font, text, position, Color.Black, rotation, origin, scale, spread);
        spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, 0, 0);
    }

    public static void DrawStringShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) {
        foreach (Vector2 v in ChatManager.ShadowDirections) {
            spriteBatch.DrawString(font, text, position + v * spread, color, rotation, origin, scale, 0, 0);
        }
    }

}