using TText = SpikysLib.Configs.Text;
using TDictionaryValuesElement = SpikysLib.Configs.UI.DictionaryValuesElement;

namespace SpikysLib.Reflection;

internal static class DictionaryValuesElement {
    public static readonly Field<TDictionaryValuesElement, TText> _unloaded = new(nameof(_unloaded));
}