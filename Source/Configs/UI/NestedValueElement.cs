using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class NestedValueElement : ConfigElement<IKeyValuePair> {
    public override void OnBind() {
        base.OnBind();

        IKeyValuePair value = Value;

        KeyValueWrapperAttribute? customWrapperAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<KeyValueWrapperAttribute>(MemberInfo, Item, List);
        _wrapper = KeyValueWrapper.CreateWrapper(
            new(() => value.Key, v => value.Key = v), new(() => value.Value, v => value.Value = v),
            customWrapperAttribute?.Type
        );

        int top = 0;
        (_containerValue, UIElement uiValue) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetValueMember(_wrapper.GetType()), _wrapper, 0);
        _uiValue = (ConfigElement)uiValue;
        if (uiValue is Terraria.ModLoader.Config.UI.ObjectElement objectElement) {
            _containerValue.Left.Pixels -= 20;
            _containerValue.Width.Pixels += 20;
            objectElement.expandButton.Left.Set(-25f, 1f);
        } else _expanded = true;

        top = 0;
        (UIElement conParent, UIElement uiParent) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetKeyMember(_wrapper.GetType()), _wrapper, 0);
        _uiParent = (ConfigElement)uiParent;
        conParent.Left.Pixels -= 20;
        conParent.Width.Pixels -= 5;
        _uiParent.OnLeftDoubleClick += (_, _) => Expanded = !Expanded;

        if (_uiValue is not Terraria.ModLoader.Config.UI.ObjectElement) {
            _containerValue.Top = conParent.Height;
            _expandButton = new(ExpandedTexture, Language.GetTextValue("tModLoader.ModConfigCollapse"));
            _expandButton.Top.Set(4f, 0f);
            _expandButton.Left.Set(-25f, 1f);
            _expandButton.OnLeftClick += (a, b) => Expanded = !Expanded;
            Append(_expandButton);
        }

        TextDisplayFunction = () => $"{Label}{_uiParent.TextDisplayFunction()[nameof(IKeyValuePair.Key).Length..]}"; // In case the parent has a custom label with added fluff
        _uiParent.DrawLabel = false;
        _uiValue.DrawLabel = false;
        _uiValue.TextDisplayFunction = () => string.Empty;

        Func<string> parentTooltip = _uiParent.TooltipFunction;
        Func<string> valueTooltip = _uiValue.TooltipFunction;
        _uiParent.TooltipFunction = null;
        _uiValue.TooltipFunction = null;

        _uiParent.backgroundColor = Color.Transparent;
        _uiValue.backgroundColor = Color.Transparent;

        _wrapper.OnBind(_uiValue);

        Expanded = false;
    }

    public override void Recalculate() {
        base.Recalculate();
        if (_uiValue is Terraria.ModLoader.Config.UI.ObjectElement) Height.Pixels = Math.Max(_uiValue.Height.Pixels, _uiParent.Height.Pixels);
        else {
            Height.Pixels = _uiParent.Height.Pixels;
            _containerValue.Top = _uiParent.Height;
            if (Expanded) Height.Pixels = Math.Max(Height.Pixels, _uiValue.Height.Pixels + _containerValue.Top.Pixels);
        }
        if (Parent is not null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    public bool Expanded {
        get => _uiValue is Terraria.ModLoader.Config.UI.ObjectElement objectElement ? objectElement.expanded : _expanded;
        set {
            if (_uiValue is Terraria.ModLoader.Config.UI.ObjectElement objectElement) {
                objectElement.expanded = value;
                objectElement.pendingChanges = true;
                return;
            }

            if (_expanded = value) {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigCollapse");
                _expandButton.SetImage(ExpandedTexture);
                Append(_containerValue);
            } else {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigExpand");
                _expandButton.SetImage(CollapsedTexture);
                RemoveChild(_containerValue);
            }
            Recalculate();
        }
    }

    private bool _expanded; // Only used if _isObjectElement is false
    private UIHoverImage _expandButton = null!;
    private UIElement _containerValue = null!;

    private ConfigElement _uiParent = null!;
    private ConfigElement _uiValue = null!;

    private IKeyValueWrapper _wrapper = null!;
}