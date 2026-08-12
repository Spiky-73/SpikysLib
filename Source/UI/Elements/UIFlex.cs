using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SpikysLib.UI.Elements;

public class UIFlexGrid : UIGrid {

    public UIFlexGrid() : base() { }
    [Obsolete("use UIFlexGrid() instead", true)] // v1.4
    public UIFlexGrid(int itemsPerLine) : base() { ItemsPerLine = itemsPerLine; }

    /// <summary>
    /// Determine the number of items per line. When set to 0, Uses the `Width` property
    /// </summary>
    public int ItemsPerLine { get; set; } = 0;
    public bool FlexHeight = true;

    [Obsolete("set `ItemsPerLine` to 0 to disable instead", true)] // v4
    public bool FlexWidth = true;

    public override void Recalculate() {
        float maxWidth = 0;
        float totalHeight = 0;
        if (ItemsPerLine > 0) {
            for (int l = 0; l < _items.Count; l += ItemsPerLine) {
                float lineWidth = 0;
                float lineHeight = 0;
                for (int c = 0; c < ItemsPerLine && l + c < _items.Count; c++) {
                    lineWidth += _items[l + c].Width.Pixels;
                    if (_items[l + c].Height.Pixels > lineHeight) lineHeight = _items[l + c].Height.Pixels;
                }
                if (lineWidth > maxWidth) maxWidth = lineWidth;
                totalHeight += lineHeight;
            }
            Width.Set(maxWidth + (ItemsPerLine - 1) * ListPadding, 0);
        }
        if (FlexHeight) {
            CalculatedStyle parentDimensions = (Parent == null) ? UserInterface.ActiveInstance.GetDimensions() : Parent.GetInnerDimensions();
            if (Parent != null && Parent is UIList) parentDimensions.Height = float.MaxValue;
            CalculatedStyle calculatedStyle = GetDimensionsBasedOnParentDimensions(parentDimensions);
            calculatedStyle.Width -= MarginLeft + MarginRight + PaddingLeft + PaddingRight;
            float width = calculatedStyle.Width;
            float height = 0f;
            float lineWidth = 0f;
            float lineHeight = 0f;
            foreach (var item in _items) {
                var outerDimensions = item.GetOuterDimensions();
                if (lineWidth + outerDimensions.Width > width && lineWidth > 0f) {
                    height += lineHeight + ListPadding;
                    lineWidth = 0f;
                    lineHeight = 0f;
                }
                lineHeight = Math.Max(lineHeight, outerDimensions.Height);
                item.Left.Set(lineWidth, 0f);
                lineWidth += outerDimensions.Width + ListPadding;
                item.Top.Set(height, 0f);
            }
            Height.Set(height + lineHeight, 0);
        }
        base.Recalculate();
    }
}

public class UIFlexList : UIList {
    public bool FlexHeight = true;
    public bool FlexWidth = false;
    public override void Recalculate() {
        if (FlexWidth) Width.Set(_items.Count == 0 ? 0 : _items.Select(i => i.Width.Pixels).Max(), 0);
        if (FlexHeight) {
            float height = _items.Select(i => i.Height.Pixels).Sum();
            Height.Set(height + (_items.Count - 1) * ListPadding, 0);
        }
        base.Recalculate();
    }
}