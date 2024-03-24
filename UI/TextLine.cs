using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace SpikysLib.UI;

public interface ITextLine {
    string Value { get; }
    Color? Color { get; }
}
public readonly record struct LocalizedLine(LocalizedText Text, Color? Color = null, params object[] Args) : ITextLine {
    public string Value => Text.Format(Args);
}
public readonly record struct StringLine(string Value, Color? Color = null) : ITextLine;
