using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class NestedValueElement : ConfigElement<INestedValue> {
    public override void OnBind() {
        base.OnBind();

        INestedValue value = Value;

        int top = 0;
        (UIElement conValue, UIElement uiValue) = ConfigManager.WrapIt(this, ref top, new(value.GetType().GetProperty(nameof(INestedValue.Value))), value, 0);
        _uiValue = (ConfigElement)uiValue;
        conValue.Left.Pixels -= 20;
        conValue.Width.Pixels += 20;
        ((UIImage)Reflection.ObjectElement.expandButton.GetValue(_uiValue)!).Left.Set(-25f, 1f);

        top = 0;
        (UIElement conParent, UIElement uiParent) = ConfigManager.WrapIt(this, ref top, new(value.GetType().GetProperty(nameof(INestedValue.Key))), value, 0);
        _uiParent = (ConfigElement)uiParent;
        conParent.Left.Pixels -= 20;
        conParent.Width.Pixels -= 5;
        _uiParent.OnLeftDoubleClick += (_, _) => Expand();

        DrawLabel = false;
        Reflection.ConfigElement.DrawLabel.SetValue(_uiParent, false);
        Func<string> parentText = Reflection.ConfigElement.TextDisplayFunction.GetValue(_uiParent);
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_uiValue, () => $"{TextDisplayFunction()}{parentText()[nameof(INestedValue.Key).Length..]}");

        Reflection.ConfigElement.TooltipFunction.SetValue(_uiParent, TooltipFunction);
        Reflection.ConfigElement.TooltipFunction.SetValue(_uiValue, TooltipFunction);

        Reflection.ConfigElement.backgroundColor.SetValue(_uiParent, Color.Transparent);
        Reflection.ConfigElement.backgroundColor.SetValue(_uiValue, Color.Transparent);
    }

    public void Expand() {
        Reflection.ObjectElement.expanded.SetValue(_uiValue, !(bool)Reflection.ObjectElement.expanded.GetValue(_uiValue)!);
        Reflection.ObjectElement.pendingChanges.SetValue(_uiValue, true);
    }

    public override void Recalculate() {
        base.Recalculate();
        Height = _uiValue.Height;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    private ConfigElement _uiParent = null!;
    private ConfigElement _uiValue = null!;
}