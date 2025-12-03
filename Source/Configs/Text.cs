using Newtonsoft.Json;
using SpikysLib.UI;
using Terraria.ModLoader.Config;
namespace SpikysLib.Configs;

[CustomModConfigItem(typeof(UI.TextElement))]
public sealed class Text {
    public Text() {}
    public Text(ITextLine? label = null, ITextLine? tooltip = null) { Label = label; Tooltip = tooltip; }

    [JsonIgnore] public ITextLine? Label { get; }
    [JsonIgnore] public ITextLine? Tooltip { get; }
}
