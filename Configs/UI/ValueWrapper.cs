using System;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs.UI;

public class ValueWrapper<TKey, TValue> {
    public TKey Key { get; private set; } = default!;
    public virtual TValue Value { get; set; } = default!;
    public virtual void OnBind(ConfigElement element) { }

    internal void Bind(TKey key, TValue value) => (Key, Value) = (key, value);
}

public static class ValueWrapper {
    public static PropertyFieldWrapper GetValueWrapper(Type valueWrapperType)
        => new(valueWrapperType.GetProperty(nameof(ValueWrapper<object, object>.Value), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly));
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Enum)]
public sealed class ValueWrapperAttribute : Attribute {
    public ValueWrapperAttribute(Type type) {
        if (!type.IsSubclassOfGeneric(typeof(ValueWrapper<,>), out _)) throw new ArgumentException($"The type {type} does derive from {typeof(ValueWrapper<,>)}");
        if (type.GetGenericArguments().Length > 2) throw new ArgumentException($"The type {type} can have at most 2 generic arguments");
        Type = type;
    }

    public Type Type { get; }
}
