using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace SpikysLib.UI.Elements;

public class UIFlexGrid : UIGrid {

    public int ItemsPerLine { get; set; }
    public bool FlexHeight = true;
    public bool FlexWidth = true;

    public override void Recalculate() {
        base.Recalculate();

        if (FlexWidth) {
            float maxWidth = 0;
            for (int l = 0; l < _items.Count; l+= ItemsPerLine){
                float lineWidth = 0;
                for (int c = 0; c < ItemsPerLine && l+c < _items.Count; c += 1) lineWidth += _items[l+c].Width.Pixels;
                if (lineWidth > maxWidth) maxWidth = lineWidth;
            }

            Width.Set(maxWidth +  (ItemsPerLine - 1) * ListPadding, 0);
        }
        if (FlexHeight) Height.Set(GetTotalHeight(), 0);
    }
}

public class UIFlexList : UIList {
    public bool FlexHeight = true;
    public bool FlexWidth = true;
    public override void Recalculate() {
        base.Recalculate();
        if (FlexWidth) Width.Set(_items.Count == 0 ? 0 : _items.Select(i => i.Width.Pixels).Max(), 0);
        if (FlexHeight) Height.Set(GetTotalHeight(), 0);
    }
}