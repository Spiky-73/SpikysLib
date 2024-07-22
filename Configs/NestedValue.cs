using System;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
namespace SpikysLib.Configs;

public interface INestedValue {
    [Obsolete($"use {nameof(Key)} instead", true)]
    object Parent { get => Key; set => Key = value; }

    object Key { get; set; }
    object Value { get; set; }
}
[JsonConverter(typeof(IO.NestedValueConverter))]
[CustomModConfigItem(typeof(UI.NestedValueElement))]
public class NestedValue<TKey, TValue> : INestedValue where TKey: notnull, new() where TValue: class, new() { // TODO custom args for Key
    public NestedValue() : this(new()) { } 
    public NestedValue(TKey? key = default, TValue? value = default) {
        Key = key ?? new();
        Value = value ?? new();
    }

    [Obsolete($"use {nameof(Key)} instead", true)]
    public TKey Parent { get => Key; set => Key = value; }
    
    public TKey Key { get; set; }
    [Expand(false, false)] public TValue Value { get; set; }

    object INestedValue.Key { get => Key; set => Key = (TKey)value!; }
    object INestedValue.Value { get => Value; set => Value = (TValue)value!; }

    public static implicit operator TKey(NestedValue<TKey, TValue> self) => self.Key;
}

public sealed class Toggle<T> : NestedValue<bool, T> where T: class, new() {
    public Toggle() : this(default) {}
    public Toggle(bool enabled = false, T? value = null) : base(enabled, value) {}
}
