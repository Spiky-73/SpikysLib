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
using Terraria.Audio;
using Newtonsoft.Json;
using Terraria.ModLoader;

namespace SpikysLib.Configs.UI;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ConstantKeys() : Attribute { }

public sealed class DictionaryElement : ConfigElement<IDictionary> {

    public override void OnBind() {
        base.OnBind();

        _keyType = MemberInfo.Type.GetGenericArguments()[0];
        _valueType = MemberInfo.Type.GetGenericArguments()[1];

        _constantKeys = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ConstantKeys>(MemberInfo, Item, List) is not null;

        var wrapperAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<KeyValueWrapperAttribute>(MemberInfo, Item, List);
        _customWrapper = wrapperAttribute?.Type;

        _expandButton = new global::SpikysLib.UI.Elements.HoverImage(CollapsedTexture, Language.GetTextValue($"tModLoader.ModConfigExpand"));
        _expandButton.Left.Set(-79, 1);
        _expandButton.Top.Set(4, 0);
        _expandButton.OnLeftClick += (_, _) => Expanded = !Expanded;
        Append(_expandButton);

        _addButton = new(PlusTexture, Language.GetTextValue("tModLoader.ModConfigAdd"));
        _addButton.Top.Set(4f, 0f);
        _addButton.Left.Set(-52f, 1f);
        _addButton.OnLeftClick += delegate (UIMouseEvent a, UIElement b) {
            SoundEngine.PlaySound(SoundID.Tink);
            try {
                object keyValue = ConfigManager.AlternateCreateInstance(_keyType)!;
                if (!_keyType.IsValueType && _keyType != typeof(string)) {
                    JsonConvert.PopulateObject("{}", keyValue, ConfigManager.serializerSettings);
                }
                object toAdd = ConfigManager.AlternateCreateInstance(_valueType)!;
                if (!_valueType.IsValueType && _valueType != typeof(string)) {
                    JsonConvert.PopulateObject("{}", toAdd, ConfigManager.serializerSettings);
                }

                Value.Add(keyValue, toAdd);
            } catch (Exception e) {
                ModContent.GetInstance<SpikysLib>().Logger.Error("Error: " + e.Message);
            }
            SetupList();
            ConfigManager.SetPendingChanges();
            Expanded = true;
        };
        Append(_addButton);

        _clearButton = new(DeleteTexture, Language.GetTextValue("tModLoader.ModConfigClear"));
        _clearButton.Top.Set(4f, 0f);
        _clearButton.Left.Set(-25f, 1f);
        _clearButton.OnLeftClick += delegate (UIMouseEvent a, UIElement b) {
            SoundEngine.PlaySound(SoundID.Tink);
            Value.Clear();
            SetupList();
            ConfigManager.SetPendingChanges();
        };
        Append(_clearButton);


        _dataList.Top = new(30, 0f);
        _dataList.Left = new(0, 0f);
        _dataList.Height = new(-5, 1f);
        _dataList.Width = new(0, 1f);
        _dataList.ListPadding = 5f;
        _dataList.PaddingBottom = -5f;
        SetupList();

        var expandAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ExpandAttribute>(MemberInfo, Item, List);
        Expanded = expandAttribute?.Expand ?? true;
    }

    public void SetupList() {
        _dataList.Clear();
        _dictWrappers.Clear();

        int unloaded = 0;

        IDictionary dict = Value;
        int top = _constantKeys ? 0 : 30;
        int i = -1;
        foreach ((object k, object? v) in dict.Items()) {
            object key = k;
            object? value = v;
            i++;
            int index = i;
            if (v is null) continue;
            if (key is EntityDefinition entity && entity.IsUnloaded) {
                unloaded++;
                continue;
            }

            IKeyValueWrapper innerWrapper = KeyValueWrapper.CreateWrapper(
                new(() => key, k => {
                    if (dict.Contains(k!)) return;
                    dict.Remove(key);
                    key = k!;
                    if (dict is IOrderedDictionary oDict) oDict.Insert(index, key, value);
                    else dict.Add(key, value);
                }), new(() => dict[key], v => value = dict[key] = v),
                _customWrapper
            );
            Wrapper wrapper = Wrapper.From(innerWrapper);
            _dictWrappers.Add(wrapper);

            UIElement container; ConfigElement element;
            if (!_constantKeys) {
                (container, UIElement e) = ConfigManager.WrapIt(_dataList, ref top, wrapper.Member, wrapper, i);
                element = (ConfigElement)e;
                element.Width.Pixels -= 10;
                element.Left.Pixels += 10;
                global::SpikysLib.UI.Elements.HoverImage deleteButton = new(DeleteTexture, Language.GetTextValue("tModLoader.ModConfigRemove")) {
                    VAlign = 0.5f,
                    Left = new(element.Left.Pixels + 2, 0f),
                };
                element.Width.Pixels -= 24;
                element.Left.Pixels += 24;
                deleteButton.OnLeftClick += delegate (UIMouseEvent a, UIElement b) {
                    dict.Remove(key);
                    SetupList();
                    ConfigManager.SetPendingChanges();
                };
                container.Append(deleteButton);
            } else {
                (container, UIElement e) = ConfigManager.WrapIt(_dataList, ref top, KeyValueWrapper.GetValueMember(innerWrapper.GetType()), innerWrapper, i);
                element = (ConfigElement)e;
                (UIElement keyContainer, UIElement uiKey) = ConfigManager.WrapIt(this, ref top, KeyValueWrapper.GetKeyMember(innerWrapper.GetType()), innerWrapper, i);
                Func<string> label = Reflection.ConfigElement.TextDisplayFunction.GetValue((ConfigElement)uiKey);
                Func<string> tooltip = Reflection.ConfigElement.TooltipFunction.GetValue((ConfigElement)uiKey);
                RemoveChild(keyContainer);
                Reflection.ConfigElement.TextDisplayFunction.SetValue(element, key switch {
                    ItemDefinition item => () => $"[i:{item.Type}] {item.Name}",
                    IEntityDefinition def => () => def.DisplayName,
                    _ => () => {
                        string l = label();
                        return l.StartsWith("Key: ") ? l[(nameof(IKeyValuePair.Key).Length + 2)..] : key.ToString() ?? "";
                    }
                });
                Reflection.ConfigElement.TooltipFunction.SetValue(element, key switch {
                    IEntityDefinition def => () => def.Tooltip ?? string.Empty,
                    _ => tooltip
                });
            }
            if (dict is IOrderedDictionary) {
                global::SpikysLib.UI.Elements.HoverImageSplit moveButton = new(UpDownTexture, Language.GetTextValue($"{Localization.Keys.UI}.Up"), Language.GetTextValue($"{Localization.Keys.UI}.Down")) {
                    VAlign = 0.5f,
                    Left = new(element.Left.Pixels + 2, 0f)
                };
                element.Width.Pixels -= 24;
                element.Left.Pixels += 24;
                moveButton.OnLeftClick += (UIMouseEvent a, UIElement b) => {
                    if (moveButton.HoveringUp ? index <= 0 : index >= dict.Count - 1) return;
                    ((IOrderedDictionary)Value).Move(index, index + (moveButton.HoveringUp ? -1 : 1));
                    SetupList();
                    ConfigManager.SetPendingChanges();
                };
                container.Append(moveButton);
            }
            innerWrapper.OnBind(element);
        }
        if (unloaded > 0) {
            _unloaded = new(new LocalizedLine(Language.GetText($"{Localization.Keys.UI}.Unloaded"), Colors.RarityTrash, unloaded));
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, new(Reflection.DictionaryElement._unloaded), this, i);
        }
        MaxHeight.Pixels = int.MaxValue;
        Recalculate();
    }

    public override void Recalculate() {
        base.Recalculate();
        int defaultHeight = _constantKeys ? 0 : 30;
        if (_dataList.Count > 1) defaultHeight -= 5;
        float h = (_dataList.Parent != null) ? (_dataList.GetTotalHeight() + defaultHeight) : defaultHeight;
        Height.Set(h, 0f);
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(h, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        if (_constantKeys) {
            DrawChildren(spriteBatch);
            return;
        }
        base.Draw(spriteBatch);
    }

    public bool Expanded {
        get => _expanded;
        set {
            if (_expanded = value) {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigCollapse");
                _expandButton.SetImage(CollapsedTexture);
                Append(_dataList);
            } else {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigExpand");
                _expandButton.SetImage(ExpandedTexture);
                RemoveChild(_dataList);
            }
            Recalculate();
        }
    }

    private bool _expanded;
    private global::SpikysLib.UI.Elements.HoverImage _expandButton = null!;
    private global::SpikysLib.UI.Elements.HoverImage _addButton = null!;
    private global::SpikysLib.UI.Elements.HoverImage _clearButton = null!;
    private Type _keyType = null!;
    private Type _valueType = null!;

    private Text _unloaded = null!;

    private Type? _customWrapper;
    private readonly List<Wrapper> _dictWrappers = [];
    private readonly UIList _dataList = [];

    private bool _constantKeys;
}