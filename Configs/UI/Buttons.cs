using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace SpikysLib.Configs.UI;

[Obsolete("use SpikysLib.UI.Elements.HoverImage instead", true)] // v1.3
public sealed class HoverImage : global::SpikysLib.UI.Elements.HoverImage {
    public HoverImage(Asset<Texture2D> texture, string hoverText) : base(texture, hoverText) {}
}

[Obsolete("use SpikysLib.UI.Elements.HoverImageSplit instead", true)] // v1.3
public sealed class HoverImageSplit : global::SpikysLib.UI.Elements.HoverImageSplit {
    public HoverImageSplit(Asset<Texture2D> texture, string hoverTextUp, string hoverTextDown) : base(texture, hoverTextUp, hoverTextDown) {}
}
