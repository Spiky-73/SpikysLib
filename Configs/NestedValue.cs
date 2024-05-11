using Terraria.ModLoader.Config;
namespace SpikysLib.Configs;

public interface INestedValue {
    public object Parent { get; set; }
    public object Value { get; set; }
}

[CustomModConfigItem(typeof(UI.NestedValueElement))]
public class NestedValue<TParent, TValue> : INestedValue where TParent: struct where TValue: class, new() { // TODO custom args for Parent
    public NestedValue() : this(default) { } 
    public NestedValue(TParent parent = default, TValue? value = default) {
        Parent = parent;
        Value = value ?? new();
    }

    public TParent Parent { get; set; }

    [Expand(false, false)]
    public TValue Value { get; set; }

    object INestedValue.Parent { get => Parent; set => Parent = (TParent)value!; }
    object INestedValue.Value { get => Value; set => Value = (TValue)value!; }

    public static implicit operator TParent(NestedValue<TParent, TValue> self) => self.Parent;
}

public sealed class Toggle<T> : NestedValue<bool, T> where T: class, new() {
    public Toggle() : this(default) {}
    public Toggle(bool enabled = false, T? value = null) : base(enabled, value) {}
}
