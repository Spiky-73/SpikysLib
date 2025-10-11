using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.Localization;
using SpikysLib.Collections;
using SpikysLib.UI;
using Terraria.ID;

namespace SpikysLib.Configs.UI;

[Obsolete("use DictonaryElement instead")] // v1.4
public sealed class DictionaryValuesElement : ConfigElement<IDictionary> {

    public override void OnBind() {
        base.OnBind();

        _dataList.Top = new(0, 0f);
        _dataList.Left = new(0, 0f);
        _dataList.Height = new(-5, 1f);
        _dataList.Width = new(0, 1f);
        _dataList.ListPadding = 5f;
        _dataList.PaddingBottom = -5f;
        SetupList();

        Append(_dataList);
    }

    public void SetupList() {
        _dataList.Clear();
        _dictWrappers.Clear();

        int unloaded = 0;

        var wrapperAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<KeyValueWrapperAttribute>(MemberInfo, Item, List);
        Type? customWrapper = wrapperAttribute?.Type;

        IDictionary dict = Value;
        int top = 0;
        int i = -1;
        foreach ((object key, object? value) in dict.Items()) {
            i++;
            if (value is null) continue;
            if (key is EntityDefinition entity && entity.IsUnloaded) {
                unloaded++;
                continue;
            }

            IKeyValueWrapper wrapper = KeyValueWrapper.CreateWrapper(
                new(() => key, _ => throw new NotSupportedException()), new(() => dict[key], v => dict[key] = v),
                customWrapper
            );
            _dictWrappers.Add(wrapper);
            (UIElement container, UIElement e) = ConfigManager.WrapIt(_dataList, ref top, KeyValueWrapper.GetValueMember(wrapper.GetType()), wrapper, i);
            ConfigElement element = (ConfigElement)e;

            if (dict is IOrderedDictionary) {
                element.Width.Pixels -= 25;
                element.Left.Pixels += 25;

                int index = i;
                global::SpikysLib.UI.Elements.HoverImageSplit moveButton = new(UpDownTexture, Language.GetTextValue($"{Localization.Keys.UI}.Up"), Language.GetTextValue($"{Localization.Keys.UI}.Down")) {
                    VAlign = 0.5f,
                    Left = new(2, 0f),
                };
                moveButton.OnLeftClick += (UIMouseEvent a, UIElement b) => {
                    if (moveButton.HoveringUp ? index <= 0 : index >= dict.Count - 1) return;
                    ((IOrderedDictionary)Value).Move(index, index + (moveButton.HoveringUp ? -1 : 1));
                    SetupList();
                    ConfigManager.SetPendingChanges();
                };
                container.Append(moveButton);
            }

            (UIElement keyContainer, UIElement uiKey) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetKeyMember(wrapper.GetType()), wrapper, i);
            Func<string> label = Reflection.ConfigElement.TextDisplayFunction.GetValue((ConfigElement)uiKey);
            Func<string> tooltip = Reflection.ConfigElement.TooltipFunction.GetValue((ConfigElement)uiKey);
            RemoveChild(keyContainer);
            Reflection.ConfigElement.TextDisplayFunction.SetValue(element, key switch {
                ItemDefinition item => () => $"[i:{item.Type}] {item.Name}",
                IEntityDefinition def => () => def.DisplayName,
                _ =>  () => {
                    string l = label();
                    return l.StartsWith("Key: ") ? l[(nameof(IKeyValuePair.Key).Length + 2)..] : key.ToString() ?? "";
                }
            });
            Reflection.ConfigElement.TooltipFunction.SetValue(element, key switch {
                IEntityDefinition def => () => def.Tooltip ?? string.Empty,
                _ => tooltip
            });
            wrapper.OnBind(element);
        }
        if (unloaded > 0) {
            _unloaded = new(new LocalizedLine(Language.GetText($"{Localization.Keys.UI}.Unloaded"), Colors.RarityTrash, unloaded));
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, new(Reflection.DictionaryValuesElement._unloaded), this, i);
        }
        MaxHeight.Pixels = int.MaxValue;
        Recalculate();
    }

    public override void Recalculate() {
        base.Recalculate();
        int defaultHeight = _dataList.Count > 1 ? -5 : 0;
        float h = (_dataList.Parent != null) ? (_dataList.GetTotalHeight() + defaultHeight) : defaultHeight;
        Height.Set(h, 0f);
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(h, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch) => DrawChildren(spriteBatch);

    private Text _unloaded = null!;

    private readonly List<IKeyValueWrapper> _dictWrappers = [];
    private readonly UIList _dataList = [];
}