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
using SpikysLib.Extensions;
using SpikysLib.UI;
using Terraria.ID;

namespace SpikysLib.Configs.UI;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Enum)]
public sealed class ValueWrapperAttribute : Attribute {
    public ValueWrapperAttribute(Type type) {
        if (!type.IsSubclassOfGeneric(typeof(ValueWrapper<,>), out _)) throw new ArgumentException($"The type {type} does derive from {typeof(ValueWrapper<,>)}");
        if (type.GetGenericArguments().Length > 2) throw new ArgumentException($"The type {type} can have at most 2 generic arguments");
        Type = type;
    }

    public Type Type { get; }
}


public class ValueWrapper<TKey, TValue> where TKey : notnull {
    public TKey Key { get; private set; } = default!;
    public TValue Value { get => (TValue)Dict[Key]!; set => Dict[Key] = value; }
    public virtual void OnBind(ConfigElement element) {}

    internal void Bind(IDictionary dict, TKey key) => (Dict, Key) = (dict, key);
    public IDictionary Dict { get; private set; } = null!;
}

public sealed class DictionaryValuesElement : ConfigElement<IDictionary> {

    public override void OnBind() {
        base.OnBind();

        if (Value is null) throw new ArgumentNullException("This config element only supports IDictionaries");

        ValueWrapperAttribute? customWrapper = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ValueWrapperAttribute>(MemberInfo, Item, List);
        if (customWrapper is not null) _wrapperType = customWrapper.Type;

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
            List<Type> args = new(2);
            if (_wrapperType.GetGenericArguments().Length > 1) args.Add(key.GetType());
            if (_wrapperType.GetGenericArguments().Length > 0) args.Add(value.GetType());
            object wrapper = Activator.CreateInstance(_wrapperType.MakeGenericType([.. args]))!;
            wrapper.Call(nameof(ValueWrapper<object, object>.Bind), dict, key);
            _dictWrappers.Add(wrapper);
            (UIElement container, UIElement e) = ConfigManager.WrapIt(_dataList, ref top, new(wrapper.GetType().GetProperty(nameof(ValueWrapper<object, object>.Value), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly)), wrapper, i);
            ConfigElement element = (ConfigElement)e;

            if (dict is IOrderedDictionary) {
                element.Width.Pixels -= 25;
                element.Left.Pixels += 25;

                int index = i;
                HoverImageSplit moveButton = new(UpDownTexture, Language.GetTextValue($"{Localization.Keys.UI}.Up"), Language.GetTextValue($"{Localization.Keys.UI}.Down")) {
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

            string name = key switch {
                IEntityDefinition preset => preset.DisplayName,
                ItemDefinition item => $"[i:{item.Type}] {item.Name}",
                _ => key.ToString()!
            };
            Reflection.ConfigElement.TextDisplayFunction.SetValue(element, () => name);
            wrapper.Call(nameof(ValueWrapper<object, object>.OnBind), element);
        }
        if (unloaded > 0) {
            _unloaded = new(new LocalizedLine(Language.GetText("Mods.ModLoader.Unloaded"), Colors.RarityTrash, unloaded));
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

    private readonly List<object> _dictWrappers = [];
    private Type _wrapperType = typeof(ValueWrapper<,>);
    private readonly UIList _dataList = [];
}