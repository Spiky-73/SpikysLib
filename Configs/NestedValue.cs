using Newtonsoft.Json;
using SpikysLib.Configs.UI;
using Terraria.ModLoader.Config;
namespace SpikysLib.Configs;

public interface IKeyValuePair {
    object? Key { get; set; }
    object? Value { get; set; }
}

public interface IKeyValuePair<TKey, TValue> : IKeyValuePair {
    new TKey Key { get; set; }
    new TValue Value { get; set; }

    object? IKeyValuePair.Key { get => Key; set => Key = (TKey)value!; }
    object? IKeyValuePair.Value { get => Value; set => Value = (TValue)value!; }
}

// TODO test if default and not new()
[JsonConverter(typeof(IO.NestedValueConverter))]
[CustomModConfigItem(typeof(NestedValueElement))]
public class NestedValue<TKey, TValue> : IKeyValuePair<TKey, TValue> where TKey: new() where TValue: new() {
    public NestedValue() : this(new()) { } 
    public NestedValue(TKey? key = default, TValue? value = default) {
        Key = key ?? new();
        Value = value ?? new();
    }
    
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    public static implicit operator TKey(NestedValue<TKey, TValue> self) => self.Key;
}