using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class AnyElement : ConfigElement<object> {

    public override void OnBind() {
        base.OnBind();
        object value = Value;
        _wrapper = Wrapper.From(value);

        int top = 0;
        PropertyFieldWrapper member = _wrapper.Member;
        (UIElement container, UIElement element) = ConfigManager.WrapIt(this, ref top, member, _wrapper, 0);
        _element = (ConfigElement)element;
        container.Left.Pixels -= 20;
        container.Width.Pixels += 20;

        _element.backgroundColor = Color.Transparent;
        var childText = _element.TextDisplayFunction;
        _element.TextDisplayFunction = () => $"{TextDisplayFunction()}{childText()[member.Name.Length..]}";
        _element.TooltipFunction = TooltipFunction;
        DrawLabel = false;
        TooltipFunction = null;
        MaxHeight.Pixels = int.MaxValue;
        Recalculate();
    }

    public override void Recalculate() {
        base.Recalculate();
        Height = _element.Height;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    private Wrapper _wrapper = null!;
    private ConfigElement _element = null!;
}
