using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace SpikysLib.UI;

public interface ITextLine {
    string Value { get; }
    Color? Color { get; }
}
public readonly record struct LocalizedLine(LocalizedText Text, Color? Color, params object[] Args) : ITextLine {
    public LocalizedLine(LocalizedText Text, params object[] Args) : this(Text, null, Args) { }
    public string Value => Text.Format(Args);
}
public readonly record struct StringLine(string Value, Color? Color = null) : ITextLine;
