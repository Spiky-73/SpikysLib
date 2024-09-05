namespace SpikysLib.Configs;

public interface IToggle: IKeyValuePair {
    new bool Key { get; set; }
}

public sealed class Toggle<T> : NestedValue<bool, T>, IToggle where T: new() {
    public Toggle() : this(default) {}
    public Toggle(bool enabled = false, T? value = default) : base(enabled, value) {}
}
