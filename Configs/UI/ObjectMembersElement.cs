using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Newtonsoft.Json;
using Terraria.Localization;

namespace SpikysLib.Configs.UI;

public sealed class ObjectMembersElement : ConfigElement<object> {

    public override void OnBind() {
        base.OnBind();

        _dataList = new() {
            Top = new(0, 0f),
            Left = new(0, 0f),
            Height = new(-5, 1f),
            Width = new(0, 1f),
            ListPadding = 5f,
            PaddingBottom = -5f
        };
        SetupList();

        _expandButton = new HoverImage(CollapsedTexture, Language.GetTextValue($"tModLoader.ModConfigExpand")) {
            Left = new(-30 + 5, 1),
            Top = new(4, 0)
        };
        _expandButton.OnLeftClick += (_, _) => Expanded = !Expanded;

        ExpandAttribute? expandAttribute = ConfigManager.GetCustomAttributeFromMemberThenMemberType<ExpandAttribute>(MemberInfo, Item, List);
        if (expandAttribute != null) Expanded = expandAttribute.Expand;
    }

    public bool Expanded {
        get => _expanded;
        set {
            if(_expanded = value) {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigExpand");
                _expandButton.SetImage(ExpandedTexture);
                RemoveChild(_dataList);
            } else {
                _expandButton.HoverText = Language.GetTextValue($"tModLoader.ModConfigCollapse");
                _expandButton.SetImage(CollapsedTexture);
                Append(_dataList);
            }
            Recalculate();
        }
    }

    public void SetupList() {
        _dataList.Clear();

        object value = Value;
        int order = 0;
        foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(value)) {
            if (!Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute))) {
                int top = 0;
                object[] args = [_dataList, top, order, variable];
                Reflection.UIModConfig.HandleHeader.Invoke(args);
                top = (int)args[1]; order = (int)args[2];
                (UIElement container, UIElement e) = ConfigManager.WrapIt(_dataList, ref top, variable, value, order++);
                container.Left.Pixels -= 20;
                container.Width.Pixels += 20;
            }
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

    private bool _expanded;
    private HoverImage _expandButton = null!;
    private UIList _dataList = null!;
}