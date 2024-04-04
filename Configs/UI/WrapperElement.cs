using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class WrapperElement : ConfigElement<Wrapper> {

    public override void OnBind() {
        base.OnBind();
        Wrapper wrapper = Value;

        int top = 0;
        PropertyFieldWrapper member = wrapper.Member;
        (UIElement container, UIElement element) = ConfigManager.WrapIt(this, ref top, member, wrapper, 0);
        _element = (ConfigElement)element;
        container.Left.Pixels -= 20;
        container.Width.Pixels += 20;

        Reflection.ConfigElement.backgroundColor.SetValue(_element, Color.Transparent);
        Func<string> childText = Reflection.ConfigElement.TextDisplayFunction.GetValue(_element)!;
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_element, wrapper.Member.Name == nameof(Wrapper.Value) ? () => $"{TextDisplayFunction()}{childText()[member.Name.Length..]}" : () => $"{TextDisplayFunction()} ({childText()})");
        Reflection.ConfigElement.TooltipFunction.SetValue(_element, TooltipFunction);
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

    private ConfigElement _element = null!;
}
