using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace SpikysLib.UI.Elements;

public class UIFlexGrid : UIGrid {

    [Obsolete("use UIFlexGrid(int itemsPerLine) instead", true)] // v1.3.1.1
    public UIFlexGrid() : base() { }
    public UIFlexGrid(int itemsPerLine) : base() { ItemsPerLine = itemsPerLine; }

    public int ItemsPerLine { get; set; }
    public bool FlexHeight = true;
    public bool FlexWidth = true;

    public override void Recalculate() {
        float maxWidth = 0;
        float totalHeight = 0;
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

        if (FlexWidth && ItemsPerLine > 0) {
            Width.Set(maxWidth + (ItemsPerLine - 1) * ListPadding, 0);
        }
        if (FlexHeight) {
            int rows = (_items.Count + ItemsPerLine - 1) / ItemsPerLine;
            Height.Set(totalHeight + ListPadding * (rows - 1), 0);
        }
        base.Recalculate();
    }
}

public class UIFlexList : UIList {
    public bool FlexHeight = true;
    public bool FlexWidth = true;
    public override void Recalculate() {
        if (FlexWidth) Width.Set(_items.Count == 0 ? 0 : _items.Select(i => i.Width.Pixels).Max(), 0);
        if (FlexHeight) {
            float height = _items.Select(i => i.Height.Pixels).Sum();
            Height.Set(height + (_items.Count - 1) * ListPadding, 0);
        }
        base.Recalculate();
    }
}