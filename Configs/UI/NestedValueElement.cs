using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
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
        (_containerValue, UIElement uiValue) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetValueWrapper(_wrapper.GetType()), _wrapper, 0);
        _uiValue = (ConfigElement)uiValue;
        _isObjectElement = uiValue.GetType() == Reflection.ObjectElement.Type;
        if (_isObjectElement) {
            _containerValue.Left.Pixels -= 20;
            _containerValue.Width.Pixels += 20;
            ((UIImage)Reflection.ObjectElement.expandButton.GetValue(_uiValue)!).Left.Set(-25f, 1f);
        } else _expanded = true;

        top = 0;
        (UIElement conParent, UIElement uiParent) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetKeyWrapper(_wrapper.GetType()), _wrapper, 0);
        _uiParent = (ConfigElement)uiParent;
        conParent.Left.Pixels -= 20;
        conParent.Width.Pixels -= 5;
        _uiParent.OnLeftDoubleClick += (_, _) => Expanded = !Expanded;

        if (!_isObjectElement) {
            _containerValue.Top = conParent.Height;
            _expandButton = new HoverImage(ExpandedTexture, Language.GetTextValue("tModLoader.ModConfigCollapse"));
            _expandButton.Top.Set(4f, 0f);
            _expandButton.Left.Set(-25f, 1f);
            _expandButton.OnLeftClick += (a, b) => Expanded = !Expanded;
            Append(_expandButton);
        }

        DrawLabel = false;
        Func<string> parentText = Reflection.ConfigElement.TextDisplayFunction.GetValue(_uiParent);
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_uiParent, () => $"{TextDisplayFunction()}{parentText()[nameof(IKeyValuePair.Key).Length..]}");
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_uiValue, () => string.Empty);

        Reflection.ConfigElement.TooltipFunction.SetValue(_uiParent, () => TooltipFunction());
        Reflection.ConfigElement.TooltipFunction.SetValue(_uiValue, () => TooltipFunction());

        Reflection.ConfigElement.backgroundColor.SetValue(_uiParent, Color.Transparent);
        Reflection.ConfigElement.backgroundColor.SetValue(_uiValue, Color.Transparent);

        _wrapper.OnBindKey(_uiValue);
        _wrapper.OnBind(_uiValue);

        Expanded = false;
    }

    public override void Recalculate() {
        base.Recalculate();
        if (_isObjectElement) Height = _uiValue.Height;
        else {
            Height = _uiParent.Height;
            if (_expanded) Height.Pixels += _uiValue.Height.Pixels + 5;
        }
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    public bool Expanded {
        get => _isObjectElement ? (bool)Reflection.ObjectElement.expanded.GetValue(_uiValue)! : _expanded;
        set {
            if (_isObjectElement) {
                Reflection.ObjectElement.expanded.SetValue(_uiValue, value);
                Reflection.ObjectElement.pendingChanges.SetValue(_uiValue, true);
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

    private bool _isObjectElement;
    private bool _expanded; // Only used if _isObjectElement is false
    private HoverImage _expandButton = null!;
    private UIElement _containerValue = null!;

    private ConfigElement _uiParent = null!;
    private ConfigElement _uiValue = null!;

    private IKeyValueWrapper _wrapper = null!;
}