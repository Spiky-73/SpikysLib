namespace SpikysLib.Configs;

public interface IToggle {
    bool Key { get; set; }
    object Value { get; set; }
}

public sealed class Toggle<T> : NestedValue<bool, T>, IToggle where T: new() {
    public Toggle() : this(default) {}
    public Toggle(bool enabled = false, T? value = default) : base(enabled, value) {}

    object IToggle.Value { get => Value; set => Value = (T)value!; }
}
