using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpikysLib;

[Obsolete($"use {nameof(GraphicsHelper)} instead", true)]
public static class Graphics {

    public static void DrawTileFrame(SpriteBatch spriteBatch, int tile, Vector2 position, Vector2 origin, float scale) => GraphicsHelper.DrawTileFrame(spriteBatch, tile, position, origin, scale);
}