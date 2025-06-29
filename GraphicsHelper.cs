using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.UI.Chat;

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

    public static void DrawBorderStringFourWay(this SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 position, Color textColor, Color borderColor, Vector2 origin, float scale = 1f) {
        Color color = borderColor;
        float delta = 2 * scale;
        for (int i = 0; i < 5; i++) {
            Vector2 zero = position;
            switch (i) {
            case 0:
                zero.X -= delta;
                break;
            case 1:
                zero.X += delta;
                break;
            case 2:
                zero.Y -= delta;
                break;
            case 3:
                zero.Y += delta;
                break;
            default:
                color = textColor;
                break;
            }
            DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, zero, color, 0f, origin, scale, 0, 0f);
        }
    }

    public static void DrawStringWithShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) {
        GraphicsHelper.DrawStringShadow(spriteBatch, font, text, position, Color.Black, rotation, origin, scale, spread);
        Main.spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, 0, 0);
    }

    public static void DrawStringShadow(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float spread = 2f) {
        foreach (Vector2 v in ChatManager.ShadowDirections) {
            spriteBatch.DrawString(font, text, position + v * spread, color, rotation, origin, scale, 0, 0);
        }
    }
}