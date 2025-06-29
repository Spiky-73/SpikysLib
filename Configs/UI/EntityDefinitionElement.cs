using System;
using System.Collections.Generic;
using SpikysLib.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class EntityDefinitionElement : ConfigElement<IEntityDefinition> {
    public override void OnBind() {
        base.OnBind();
        IEntityDefinition definition = Value;

        _values = definition.GetValues();

        _values = definition.GetValues();
        _index = _values.IndexOf(definition);

        Func<string> label = TextDisplayFunction;
        TextDisplayFunction = () => $"{label()}: {(_index == -1 ? Language.GetTextValue($"{Localization.Keys.UI}.None") : _values[_index].DisplayName)}";
        OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => {
            if (_expanded) CloseDropDownField(_index);
            else OpenDropDownField();
        };
        _dataList.Top = new(30, 0f);
        _dataList.Left = new(7, 0f);
        _dataList.Height = new(-7, 1f);
        _dataList.Width = new(-7 * 2, 1f);
        _dataList.ListPadding = 7f;
        MaxHeight.Pixels = int.MaxValue;

        SetupList();

        _expandButton = new global::SpikysLib.UI.Elements.HoverImage(CollapsedTexture, Language.GetTextValue($"tModLoader.ModConfigExpand"));
        _expandButton.Left.Set(-30 + 5, 1);
        _expandButton.Top.Set(4, 0);
        _expandButton.OnLeftClick += (_, _) => OpenDropDownField();
        if(!Value.AllowNull && Value.Type <= 0) OpenDropDownField();
        else CloseDropDownField(_index);
    }

    public void SetupList() {
        _dataList.Clear();
        _elements.Clear();
        for (int i = 0; i < _values.Count; i++) {
            Wrapper<Text> wrapper = new(new(new StringLine(_values[i].DisplayName), _values[i].Tooltip is not null ? new StringLine(_values[i].Tooltip!) : null));
            _elements.Add(wrapper);
            int top = 0;
            int index = i;
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, wrapper.Member, wrapper, index);
            container.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => CloseDropDownField(index);
        }
    }

    public void OpenDropDownField() => Expanded = true;
    public void CloseDropDownField(int index) {
        if (!Value.AllowNull && index < 0) return;
        if (_index != index) {
            MemberInfo.SetValue(Item, _values[index]);
            _index = index;
            ConfigManager.SetPendingChanges();
        }
        Expanded = false;
    }

    public override void Recalculate() {
        base.Recalculate();
        float height = (_dataList.Parent != null) ? (_dataList.GetTotalHeight() + 30) : 30;
        Height.Set(height, 0f);
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(height, 0f);
    }

    private int _index;
    private IList<IEntityDefinition> _values = null!;
    
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
    private readonly UIList _dataList = [];
    private readonly List<Wrapper<Text>> _elements = [];
}