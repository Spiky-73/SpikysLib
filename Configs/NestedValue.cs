using Newtonsoft.Json;
using Terraria.ModLoader.Config;
namespace SpikysLib.Configs;

public interface INestedValue {
    object? Key { get; set; }
    object? Value { get; set; }
}

[JsonConverter(typeof(IO.NestedValueConverter))]
[CustomModConfigItem(typeof(UI.NestedValueElement))]
public class NestedValue<TKey, TValue> : INestedValue where TKey: new() where TValue: new() {
    public NestedValue() : this(new()) { } 
    public NestedValue(TKey? key = default, TValue? value = default) {
        Key = key ?? new();
        Value = value ?? new();
    }
    
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    object? INestedValue.Key { get => Key; set => Key = (TKey)value!; }
    object? INestedValue.Value { get => Value; set => Value = (TValue)value!; }

    public static implicit operator TKey(NestedValue<TKey, TValue> self) => self.Key;
}