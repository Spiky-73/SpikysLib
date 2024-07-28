using Microsoft.Xna.Framework;
using MonoMod.Cil;
using SpikysLib.IL;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI.Chat;

namespace SpikysLib.Configs.UI;

public sealed class TextElement : ConfigElement<Text?> {

    public new Text? Value => base.Value;

    public override void OnBind() {
        base.OnBind();
        Text? value = Value;
        if (value?.Label?.Value.Length > 0) Label = Language.GetTextValue(value.Label.Value);
        if (value?.Tooltip?.Value.Length > 0) {
            string tooltip = Language.GetTextValue(value.Tooltip.Value);
            TooltipFunction = () => tooltip;
        }
    }

    public override void Recalculate() {
        base.Recalculate();
        Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, Label, new Vector2(0.8f), GetDimensions().Width + 1);
        Height.Pixels = size.Y + 30 - FontAssets.ItemStack.Value.LineSpacing;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    internal static void Load() => MonoModHooks.Modify(Reflection.ConfigElement.DrawSelf, ILTextColors);
    internal static void Unload() { }

    private static void ILTextColors(ILContext il) {
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(i => i.SaferMatchCall(typeof(ChatManager), nameof(ChatManager.DrawColorCodedStringWithShadow))) || !cursor.TryGotoPrev(MoveType.After, i => i.MatchLdloc3())) return;
        cursor.EmitLdarg0();
        cursor.EmitDelegate((Color color, ConfigElement self) => self is UI.TextElement textElem ? color.MultiplyRGBA(textElem.Value?.Label?.Color ?? Color.White) : color);
    }
}