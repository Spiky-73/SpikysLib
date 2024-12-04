using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace SpikysLib;

public static class GraphicsHelper {
    
    public static void ApplyRGB(ref this Color color, float mult) {
        color.R = (byte)(color.R * mult);
        color.G = (byte)(color.G * mult);
        color.B = (byte)(color.B * mult);
    }

    public static float DrawTexture(this SpriteBatch spriteBatch, Texture2D value, Color alpha, Vector2 position, ref float scale, float sizeLimit) {
        Rectangle frame = value.Frame(1, 1, 0, 0, 0, 0);
        if (frame.Width > sizeLimit || frame.Height > sizeLimit) scale *= (frame.Width <= frame.Height) ? (sizeLimit / frame.Height) : (sizeLimit / frame.Width);
        spriteBatch.Draw(value, position, new Rectangle?(frame), alpha, 0f, frame.Size() / 2f, scale, 0, 0f);
        return scale;
    }

    public static void DrawTileFrame(this SpriteBatch spriteBatch, int tile, Vector2 position, Vector2 origin, float scale) {
        Main.instance.LoadTiles(tile);

        TileObjectData tileObjectData = TileObjectData.GetTileData(tile, 0);
        (int width, int height, int padding) = tileObjectData is null ? (1, 1, 0) : (tileObjectData.Width, tileObjectData.Height, tileObjectData.CoordinatePadding);

        Vector2 topLeft = position - new Vector2(width, height) * 16 * origin * scale;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                spriteBatch.Draw(TextureAssets.Tile[tile].Value, topLeft + new Vector2(i * 16, j * 16) * scale, new Rectangle(i * 16 + i * padding, j * 16 + j * padding, 16, 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }

    public static void DrawMouseText(string tooltip) {
        if (Main.gameMenu || Main.ingameOptionsWindow) Reflection.UIModConfig.Tooltip.SetValue(tooltip);
        else Main.instance.MouseText(tooltip);
    }


}