using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs.UI;

public sealed class ObjectMembersElement : ConfigElement<object?> {

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
        Append(_dataList);
        SetupList();
    }

    public void SetupList() {
        _dataList.Clear();

        object? value = Value;
        if (value is not null) {
            int order = 0;
            foreach (PropertyFieldWrapper variable in ConfigHelper.GetFieldsAndProperties(value)) {
                int top = 0;
                object[] args = [_dataList, top, order, variable];
                Reflection.UIModConfig.HandleHeader.Invoke(args);
                top = (int)args[1]; order = (int)args[2];
                ConfigManager.WrapIt(_dataList, ref top, variable, value, order++);
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

    private UIList _dataList = null!;
}