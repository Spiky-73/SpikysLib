using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs.UI;

public readonly record struct Property<T>(Func<T> Get, Action<T> Set);

public interface IKeyValueWrapper : IKeyValuePair {
    [Obsolete("use OnBind instead", true)] // v1.4
    void OnBindKey(ConfigElement element) { }
    void OnBind(ConfigElement element) { }

    Property<object?> KeyProp { set; }
    Property<object?> ValueProp { set; }
}

public interface IKeyValueWrapper<TKey, TValue> : IKeyValuePair<TKey, TValue>, IKeyValueWrapper {
    new Property<TKey> KeyProp { set; }
    new Property<TValue> ValueProp { set; }

    Property<object?> IKeyValueWrapper.KeyProp { set => KeyProp = new(() => (TKey)value.Get()!, v => value.Set(v)); }
    Property<object?> IKeyValueWrapper.ValueProp { set => ValueProp = new(() => (TValue)value.Get()!, v => value.Set(v)); }
}


public class KeyValueWrapper<TKey, TValue>: IKeyValueWrapper<TKey, TValue> {
    public virtual TKey Key { get => KeyProp.Get(); set => KeyProp.Set(value); }
    public virtual TValue Value { get => ValueProp.Get(); set => ValueProp.Set(value); }

    [Obsolete("use OnBind instead", true)] // v1.4
    public virtual void OnBindKey(ConfigElement element) { }
    public virtual void OnBind(ConfigElement element) { }

    [JsonIgnore] public Property<TKey> KeyProp { get; set; }
    [JsonIgnore] public Property<TValue> ValueProp { get; set; }
}

[CustomModConfigItem(typeof(NestedValueElement))]
public class NestedKeyValueWrapper<TKey, TValue>: KeyValueWrapper<TKey, TValue> {}

public static class KeyValueWrapper {
    public static PropertyFieldWrapper GetKeyMember(Type type) => GetMember(type, nameof(IKeyValuePair.Key))!;
    public static PropertyFieldWrapper GetValueMember(Type type) => GetMember(type, nameof(IKeyValuePair.Value))!;
    private static PropertyFieldWrapper GetMember(Type type, string name) => type.GetPropertiesFields(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name == name).First();
    
    [Obsolete("use GetKeyMember instead", true)] // v1.4
    public static PropertyFieldWrapper GetKeyWrapper(Type type) => GetMember(type, nameof(IKeyValuePair.Key))!;
    [Obsolete("use GetValueMember instead", true)] // v1.4
    public static PropertyFieldWrapper GetValueWrapper(Type type) => GetMember(type, nameof(IKeyValuePair.Value))!;

    public static IKeyValueWrapper CreateWrapper(Property<object?> keyProp, Property<object?> valueProp, Type? customWrapper = null) {
        customWrapper ??= typeof(KeyValueWrapper<,>);
        if (customWrapper.IsGenericTypeDefinition) {
            List<Type> args = new(2);
            if (customWrapper.GetGenericArguments().Length > 1) args.Add(keyProp.Get()!.GetType());
            if (customWrapper.GetGenericArguments().Length > 0) args.Add(valueProp.Get()!.GetType());
            customWrapper = customWrapper.MakeGenericType([.. args]);
        }
        IKeyValueWrapper wrapper = (IKeyValueWrapper)Activator.CreateInstance(customWrapper)!;
        wrapper.KeyProp = keyProp;
        wrapper.ValueProp = valueProp;
        return wrapper;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Enum)]
public class KeyValueWrapperAttribute : Attribute {
    public KeyValueWrapperAttribute(Type type) {
        if (!type.ImplementsInterface(typeof(IKeyValueWrapper), out _)) throw new ArgumentException($"The type {type} does derive from {nameof(IKeyValueWrapper)}");
        if (type.GetGenericArguments().Length > 2) throw new ArgumentException($"The type {type} can have at most 2 generic arguments");
        Type = type;
    }

    public Type Type { get; }
}
