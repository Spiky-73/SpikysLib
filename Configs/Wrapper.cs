using System;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs;

[JsonConverter(typeof(IO.WrapperConverter))]
[CustomModConfigItem(typeof(UI.WrapperElement))]
[TypeConverter("SpikysLib.IO.WrapperStringConverter")]
public class Wrapper {
    public Wrapper() => Value = default;
    public Wrapper(object? value) => Value = value;


    [JsonIgnore] public virtual PropertyFieldWrapper Member => new(GetType().GetProperty(GetType() == typeof(Wrapper) ? nameof(SaveToEdit) : nameof(Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
    [JsonIgnore] public object? Value { get; set; }

    public Text SaveToEdit { get; set; } = new();

    public Wrapper ChangeType(Type type) {
        Type genericType = typeof(Wrapper<>).MakeGenericType(type);
        return GetType() == genericType ?
            this :
            From(genericType, Value switch {
                JObject { Count: 0 } or JValue { Value: null } => Activator.CreateInstance(type),
                JToken token => token.ToObject(type),
                _ => Convert.ChangeType(Value, type),
            });
    }


    public override bool Equals(object? obj) => Equals(Value, obj is Wrapper other ? other.Value : obj);
    public override int GetHashCode() => Value!.GetHashCode();
    public override string? ToString() => Value?.ToString();

    public static Wrapper From(object value) => From(value.GetType(), value);
    public static Wrapper From(Type type, object? value = null) => (Wrapper)Activator.CreateInstance(typeof(Wrapper<>).MakeGenericType(type), value)!;
}

public class Wrapper<T> : Wrapper where T : new() {
    public Wrapper() => Value = new();
    public Wrapper(T value) => Value = value;

    [JsonIgnore] new public T Value { get => (T)base.Value!; set => base.Value = value; }

    public static implicit operator T(Wrapper<T> wrapper) => wrapper.Value;
}
