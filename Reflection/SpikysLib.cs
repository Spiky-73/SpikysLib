using System.Collections.Generic;
using SpikysLib.Configs.UI;
using TText = SpikysLib.Configs.Text;
using TDictionaryValuesElement = SpikysLib.Configs.UI.DictionaryValuesElement;
using TDictionaryElement = SpikysLib.Configs.UI.DictionaryElement;

namespace SpikysLib.Reflection;

internal static class DictionaryValuesElement {
    public static readonly Field<TDictionaryValuesElement, TText> _unloaded = new(nameof(_unloaded));
}

internal static class DictionaryElement {
    public static readonly Field<TDictionaryElement, TText> _unloaded = new(nameof(_unloaded));
    public static readonly Field<TDictionaryElement, List<IKeyValueWrapper>> _dictWrappers = new(nameof(_dictWrappers));
}