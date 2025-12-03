using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace SpikysLib.UI.Elements;

[Obsolete("use Terraria.ModLoader.UI.UIHoverImage with an Assembly publiciser instead")] // v1.4
public class HoverImage : UIHoverImage {
    public HoverImage(Asset<Texture2D> texture, string hoverText) : base(texture, hoverText) { }
}

[Obsolete("use Terraria.ModLoader.UI.UIModConfigHoverImageSplit with an Assembly publiciser instead")] // v1.4
public class HoverImageSplit : UIModConfigHoverImageSplit {

    [Obsolete("use UIHoverImageSplit.HoveringUp instead")] // v1.4
    public bool HoveringUp => Main.mouseY < GetDimensions().Y + GetDimensions().Height / 2;

    public HoverImageSplit(Asset<Texture2D> texture, string hoverTextUp, string hoverTextDown) : base(texture, hoverTextUp, hoverTextDown) { }
}

[Obsolete("use UIHoverImageFramed instead", true)] // v1.4
public class HoverImageFramed(Asset<Texture2D> texture, Rectangle frame, string hoverText) : UIHoverImageFramed(texture, frame, hoverText) { }


public static class UIHoverImageSplit {

    public static bool HoveringUp(this UIModConfigHoverImageSplit self) => Main.mouseY < self.GetDimensions().Y + self.GetDimensions().Height / 2;
}

public class UIHoverImageFramed : UIImageFramed {

    public UIHoverImageFramed(Asset<Texture2D> texture, Rectangle frame, string hoverText) : base(texture, frame) {
        HoverText = hoverText;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        base.DrawSelf(spriteBatch);
        if (IsMouseHovering) GraphicsHelper.DrawMouseText(HoverText);
    }

    public string HoverText { get; set; }
}
