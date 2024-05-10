using Terraria.ModLoader.Config;
using System.Collections.Generic;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace SpikysLib.Configs.UI;

public sealed class MultiChoiceElement : ConfigElement<MultiChoice> {

    public override void OnBind() {
        base.OnBind();
        SetupMember();
    }

    public void SetupMember() {
        RemoveAllChildren();

        MultiChoice value = Value ??= (MultiChoice)Activator.CreateInstance(MemberInfo.Type)!;
        PropertyFieldWrapper selectedProp = value.Choices[value.ChoiceIndex];

        int top = 0;
        (UIElement container, UIElement element) = ConfigManager.WrapIt(this, ref top, selectedProp, value, 0);
        _selectedElement = (ConfigElement)element;
        container.Left.Pixels -= 20;
        container.Width.Pixels -= 7;

        _tooltip = TooltipFunction;
        DrawLabel = false;
        TooltipFunction = null;

        MaxHeight.Pixels = int.MaxValue;
        Reflection.ConfigElement.backgroundColor.SetValue(_selectedElement, Color.Transparent);

        Func<string> elementLabel = Reflection.ConfigElement.TextDisplayFunction.GetValue(_selectedElement)!;
        Func<string>? elementTooltip = Reflection.ConfigElement.TooltipFunction.GetValue(_selectedElement);
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_selectedElement, () => $"{TextDisplayFunction()}: {elementLabel()}");
        Reflection.ConfigElement.TooltipFunction.SetValue(_selectedElement, () => {
            List<string> parts = new();
            string? p = null;
            if ((p = _tooltip?.Invoke()) is not null && p.Length != 0) parts.Add(p);
            if ((p = elementTooltip?.Invoke()) is not null && p.Length != 0) parts.Add(p);
            return string.Join('\n', parts);
        });

        int count = value.Choices.Count;
        UIImage swapButton;
        if (count == 2) {
            swapButton = new HoverImage(PlayTexture, Language.GetTextValue($"{Localization.Keys.UI}.Change", value.Choices[(value.ChoiceIndex + 1) % count].Name));
            swapButton.OnLeftClick += (UIMouseEvent a, UIElement b) => ChangeChoice(value.ChoiceIndex + 1);
        } else {
            swapButton = new HoverImageSplit(UpDownTexture, Language.GetTextValue($"{Localization.Keys.UI}.Change", value.Choices[(value.ChoiceIndex + 1) % count].Name), Language.GetTextValue($"{Localization.Keys.UI}.Change", value.Choices[(value.ChoiceIndex - 1 + count) % count].Name));
            swapButton.OnLeftClick += (UIMouseEvent a, UIElement b) => ChangeChoice(value.ChoiceIndex + (((HoverImageSplit)swapButton).HoveringUp ? 1 : -1));
        }
        swapButton.VAlign = 0.5f;
        swapButton.Left.Set(-30 + 5, 1);
        Append(swapButton);
        Recalculate();
    }

    public void ChangeChoice(int index) {
        Value.ChoiceIndex = index;
        SetupMember();
        ConfigManager.SetPendingChanges();
    }

    public override void Recalculate() {
        base.Recalculate();
        Height = _selectedElement.Height;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    private ConfigElement _selectedElement = null!;
    private Func<string>? _tooltip = null;
}