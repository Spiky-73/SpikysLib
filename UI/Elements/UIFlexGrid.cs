using Terraria.ModLoader.UI.Elements;

namespace SpikysLib.UI.Elements;

public class UIFlexGrid : UIGrid {

    public int ItemsPerLine { get; set; }
    public int ItemWidth { get; set; }

    public override void Recalculate() {
        base.Recalculate();
        Width.Set(ItemsPerLine * (ItemWidth + ListPadding) - ListPadding, 0);
        Height.Set(GetTotalHeight(), 0);
    }
}