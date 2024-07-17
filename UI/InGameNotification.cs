using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace SpikysLib.UI;

public class InGameNotification : IInGameNotification {

    public InGameNotification(Mod mod, params ITextLine[] lines): this(new StringLine(mod.DisplayName), ModContent.Request<Texture2D>($"{mod.Name}/icon"), lines) { }
    public InGameNotification(Asset<Texture2D> icon, params ITextLine[] lines): this(null, icon, lines) { }
    public InGameNotification(ITextLine? tooltip, Asset<Texture2D> icon, params ITextLine[] lines) {
        Tooltip = tooltip;
        Icon = icon;
        Lines = lines;
    }

    public string[] GetLineValues(out Vector2 totalSize) {
        string[] lines = new string[Lines.Length];
        totalSize = Vector2.Zero;
        for (int i = 0; i < Lines.Length; i++) {
            lines[i] = Lines[i].Value;
            Vector2 size = ChatManager.GetStringSize(FontAssets.MouseText.Value, lines[i], Vector2.One);
            if (size.X > totalSize.X) totalSize.X = size.X;
            totalSize.Y += size.Y;            
        }
        return lines;
    }

    public void Update() {
        if (_fadeInTime < FadeTime / 2) _fadeInTime++;
        if (timeLeft > 0) timeLeft--;
    }

    public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition) {
        if (Opacity <= 0f) return;

        string[] lines = GetLineValues(out _panelSize);
        _panelSize += Padding*2;
        _panelSize.X += IconScale * 80 + Padding.X;
        _panelSize *= Scale;

        Rectangle panel = Utils.CenteredRectangle(bottomAnchorPosition + new Vector2(0f, (0f - _panelSize.Y) * 0.5f), _panelSize);
        bool hovering = panel.Contains(Main.MouseScreen.ToPoint());
        
        Utils.DrawInvBG(spriteBatch, panel, new Color(64, 109, 164) * (hovering ? 0.75f : 0.5f));

        spriteBatch.Draw(Icon.Value, panel.Right() - new Vector2(Padding.X, 0), null, Color.White * Opacity, 0f, new Vector2(80, 80 / 2f), IconScale * Scale, SpriteEffects.None, 0f);
        
        Vector2 position = panel.TopLeft() + Padding * Scale;
        for (int i = 0; i < lines.Length; i++) {
            Vector2 size = Utils.DrawBorderString(spriteBatch, lines[i], position, Lines[i].Color ?? new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * Opacity, Scale, 0, -0.1f);
            position.Y += size.Y;
        }
        if (hovering) OnMouseOver();
    }

    private void OnMouseOver() {
        if (PlayerInput.IgnoreMouseInterface || timeLeft <= FadeTime) return;
        Main.LocalPlayer.mouseInterface = true;
        if (Tooltip is not null && !Main.mouseText) Main.instance.MouseText(Tooltip.Value);

        if(timeLeft < 60 + FadeTime) timeLeft = 60 + FadeTime;
        
        if (!Main.mouseLeft || !Main.mouseLeftRelease) return;
        Main.mouseLeftRelease = false;
        timeLeft = FadeTime;
    }

    public void PushAnchor(ref Vector2 positionAnchorBottom) => positionAnchorBottom.Y -= _panelSize.Y * Opacity;

    public int timeLeft = 5 * 60;

    public float Scale { get {
        if (timeLeft < FadeTime) return MathHelper.Lerp(0f, 1f, (float)timeLeft / FadeTime);
        if (_fadeInTime < FadeTime/2) return MathHelper.Lerp(0f, 1f, _fadeInTime*2f / FadeTime);
        return 1;
    } }
    public float Opacity => Scale <= 0.2f ? 0f : (Scale - 0.2f) / 0.8f;
    public bool ShouldBeRemoved => timeLeft <= 0;

    public ITextLine[] Lines { get; }
    public Asset<Texture2D> Icon { get; }
    public ITextLine? Tooltip { get; }

    private Vector2 _panelSize;
    private int _fadeInTime = 0;

    public const float IconScale = 0.3f;
    public const int FadeTime = 30;
    public static readonly Vector2 Padding = new(7, 5);
}
