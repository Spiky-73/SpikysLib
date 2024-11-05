using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpikysLib.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;


public sealed class HideDefaultElement : ConfigElement<object> {

    public override void OnBind() {
        base.OnBind();

        if (Value is null) {
            object obj = Activator.CreateInstance(MemberInfo.Type, true)!;
            JsonDefaultValueAttribute jsonDefaultValueAttribute2 = JsonDefaultValueAttribute;
            JsonConvert.PopulateObject(jsonDefaultValueAttribute2?.Json ?? "{}", obj, ConfigManager.serializerSettings);
            Value = obj;
        }

        _dataList.Top = new(30f, 0f);
        _dataList.Left = new(14f, 0f);
        _dataList.Height = new(-30f, 1f);
        _dataList.Width = new(-14f, 1f);
        _dataList.ListPadding = 5f;
        _dataList.PaddingBottom = -5f;
        SetupList();

        _expandButton = new global::SpikysLib.UI.Elements.HoverImage(ExpandedTexture, Language.GetTextValue("tModLoader.ModConfigCollapse"));
        _expandButton.Top.Set(4f, 0f);
        _expandButton.Left.Set(-25f, 1f);
        _expandButton.OnLeftClick += (a, b) => Expand();
        Append(_expandButton);
        Append(_dataList);

        MaxHeight.Pixels = int.MaxValue;
        Recalculate();
    }

    public void Expand() {
        if (_expanded) {
            RemoveChild(_dataList);
            _expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigExpand");
            _expandButton.SetImage(CollapsedTexture);
        } else {
            Append(_dataList);
            _expandButton.HoverText = Language.GetTextValue("tModLoader.ModConfigCollapse");
            _expandButton.SetImage(ExpandedTexture);
        }
        _expanded = !_expanded;
        Recalculate();
    }

    public void SetupList() {
        _dataList.Clear();
        object data = Value;
        int top = 0;

        foreach (PropertyFieldWrapper variable in ConfigHelper.GetFieldsAndProperties(data)) {
            if (Equals(variable.GetValue(data), Activator.CreateInstance(variable.Type))) continue;
            _entries.Add(new(new(new StringLine(Reflection.ConfigManager.GetLocalizedLabel.Invoke(variable)), new StringLine(Reflection.ConfigManager.GetLocalizedTooltip.Invoke(variable)))));
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, _entries[^1].Member, _entries[^1], 0);
        }
    }

    public override void Recalculate() {
        base.Recalculate();
        float h = (_dataList.Parent != null) ? (_dataList.GetTotalHeight() + 30) : 30;
        Height.Set(h, 0f);
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(h, 0f);
    }

    private bool _expanded = false;
    private global::SpikysLib.UI.Elements.HoverImage _expandButton = null!;
    private readonly UIList _dataList = new();
    private readonly List<Wrapper<Text>> _entries = new();
}
