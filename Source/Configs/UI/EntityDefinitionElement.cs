using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

// Adapted from `Terraria.ModLoader.Config.UI.EnumElement2`
public class EntityDefinitionElement : ConfigElement<IEntityDefinition> {
    private int _count;
    private IList<IEntityDefinition> _values = [];
    private string[] _labels = [];
    private string?[] _tooltips = [];

    private int _hoveredIndex = -2;

    private UIAutoScaleTextTextPanel<string> _optionChoice = null!;
    private List<UIAutoScaleTextTextPanel<string>> Options = null!;
    private UIPanel _chooserPanel = null!;
    private NestedUIGrid _chooserList = null!;

    private bool _dropDown;
    private bool _cycle;

    private bool _updateNeeded;
    private bool _selectionExpanded;

    public override void OnBind() {
        base.OnBind();

        _values = Value.GetValues();
        _count = _values.Count;
        _labels = [.. _values.Select(v => v.DisplayName)];
        _tooltips = [.. _values.Select(v => v.Tooltip)];
        if (ConfigManager.GetCustomAttributeFromMemberThenMemberType<DropdownAttribute>(MemberInfo, Item, List) != null) _dropDown = true;
        else if (ConfigManager.GetCustomAttributeFromMemberThenMemberType<CycleAttribute>(MemberInfo, Item, List) != null) _cycle = true;

        if (List != null) TextDisplayFunction = () => $"{Index + 1}: ";

        _optionChoice = new(Value.DisplayName) {
            Width = new(156, 0),
            Height = new(30, 0),
            Left = new(-4, 0),
            HAlign = 1,
            PaddingBottom = 0,
            PaddingTop = 0,
            PaddingLeft = _cycle ? 6 : 36,
            PaddingRight = 6,
            UseInnerDimensions = true,
        };
        _optionChoice.OnLeftClick += (_, _) => {
            if (_cycle) {
                SetIndex((GetIndex() + 1) % _count);
                _updateNeeded = true;
            } else if (_dropDown) {
                ShowDropdown();
            } else {
                _selectionExpanded = !_selectionExpanded;
                _updateNeeded = true;
            }
        };
        _optionChoice.OnRightClick += (_, _) => {
            if (_cycle) {
                int index = GetIndex();
                SetIndex(index == -1 ? (_count - 1) : (index - 1 + _count) % _count);
                _updateNeeded = true;
            }
        };
        _optionChoice.OnUpdate += delegate (UIElement a) {
            if (a.IsMouseHovering) _hoveredIndex = GetIndex();
        };
        Append(_optionChoice);

        if (!_cycle) {
            UIImage dropdownIcon = new(UICommon.DropdownIconTexture) {
                MarginLeft = -36f,
                MarginTop = 0f,
                RemoveFloatingPointsFromDrawPosition = true,
            };
            _optionChoice.Append(dropdownIcon);
        }
        if (!_dropDown || _count > 4) {
            _chooserPanel = new() {
                Top = new(30f, 0f),
                Width = new(-8f, 1f),
                Left = new(4f, 0f),
                BackgroundColor = Color.CornflowerBlue,
                Height = new(19 + (int)Math.Ceiling(_count / 4f) * 35, 0f)
            };
        } else {
            _chooserPanel = new() {
                Width = new StyleDimension(132, 0f),
                Height = new StyleDimension(_count * 35 + 12 - 1, 0f),
                BackgroundColor = Color.CornflowerBlue
            };
        }
        _chooserList = new() {
            Height = new(30f, 1f),
            Width = new(0f, 1f)
        };
        _chooserPanel.Append(_chooserList);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        base.Draw(spriteBatch);
        if (_chooserPanel.IsMouseHovering) UIModConfig.Tooltip = "";
        if (_hoveredIndex != -2) UIModConfig.Tooltip = (_hoveredIndex != -1) ? _tooltips[_hoveredIndex] : Language.GetTextValue("tModLoader.ModConfigUnknownEnum");
    }

    private void ShowDropdown() {
        CalculatedStyle anchorButtonDimensions = _optionChoice.GetDimensions();
        _chooserPanel.Top.Set(_optionChoice.Parent.Parent.Top.Pixels + anchorButtonDimensions.Height, 0f);
        _chooserPanel.Left.Set(-4f, 0f);
        _chooserPanel.HAlign = 1f;
        _chooserPanel.SetPadding(6f);
        if (!_dropDown || _count > 4) {
            _chooserPanel.SetPadding(12f);
            _chooserPanel.Left.Set(12f, 0f);
            _chooserPanel.Width.Set(-24f, 1f);
            _chooserPanel.HAlign = 0f;
        }
        Interface.modConfig.BlockInput(_chooserPanel);
        if (Options == null) {
            Options = CreateDefinitionOptionElementList();
            _chooserList.Clear();
            _chooserList.AddRange(Options);
        }
    }

    public override void Update(GameTime gameTime) {
        _hoveredIndex = -2;
        base.Update(gameTime);
        if (_updateNeeded) {
            _updateNeeded = false;
            if (_selectionExpanded && Options == null) {
                Options = CreateDefinitionOptionElementList();
                _chooserList.Clear();
                _chooserList.AddRange(Options);
            }
            if (!_selectionExpanded) {
                _chooserPanel.MouseOut(new UIMouseEvent(_chooserPanel, new(Main.mouseX, Main.mouseY)));
                _chooserPanel.Remove();
            } else {
                Append(_chooserPanel);
            }
            float newHeight = _selectionExpanded ? (30f + _chooserPanel.Height.Pixels + 4f) : 30f;
            Height.Set(newHeight, 0f);
            if (Parent != null && Parent is UISortableElement) {
                Parent.Height.Pixels = newHeight;
            }
            _optionChoice.SetText(Value.DisplayName);
        }
    }

    private List<UIAutoScaleTextTextPanel<string>> CreateDefinitionOptionElementList() {
        List<UIAutoScaleTextTextPanel<string>> options = [];
        for (int i = 0; i < _count; i++) {
            int index = i;
            UIAutoScaleTextTextPanel<string> optionElement = new(_labels[i]);
            optionElement.Width.Set(120f, 0f);
            optionElement.Height.Set(30f, 0f);
            optionElement.OnLeftClick += delegate (UIMouseEvent a, UIElement b) {
                SetIndex(index);
                _updateNeeded = true;
                if (!_dropDown) _selectionExpanded = false;
                else Interface.modConfig.UnblockInput(a, b);
            };
            optionElement.OnUpdate += delegate (UIElement a) {
                if (a.IsMouseHovering) _hoveredIndex = index;
            };
            options.Add(optionElement);
        }
        return options;
    }

    private int GetIndex() => _values.IndexOf(Value);
    private void SetIndex(int index) {
        if (!MemberInfo.CanWrite) return;
        Value = _values[index];
        Interface.modConfig.SetPendingChanges();
    }
}
