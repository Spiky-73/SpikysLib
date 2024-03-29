using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SpikysLib.Configs.UI;

public sealed class WrapperSerializer : JsonConverter<Wrapper> {
    public override Wrapper ReadJson(JsonReader reader, Type objectType, Wrapper? existingValue, bool hasExistingValue, JsonSerializer serializer) {
        existingValue ??= (Wrapper)Activator.CreateInstance(objectType)!;
        existingValue.Value = serializer.Deserialize(reader, objectType == typeof(Wrapper) ? typeof(JToken) : existingValue.Member.Type)!;
        return existingValue;
    }
    public override void WriteJson(JsonWriter writer, [AllowNull] Wrapper value, JsonSerializer serializer) => serializer.Serialize(writer, value?.Value);
}

public sealed class WrapperStringConverter : TypeConverter {
    public WrapperStringConverter(Type type) => ParentType = type;
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType != typeof(string) && InnerConvertor.CanConvertTo(context, destinationType);
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => (sourceType == typeof(string) && InnerConvertor.CanConvertFrom(context, sourceType)) || base.CanConvertFrom(context, sourceType);
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => value is string ? base.ConvertFrom(context, culture, value) : Activator.CreateInstance(ParentType, InnerConvertor.ConvertFrom(context, culture, value));

    public Type ParentType { get; }
    public TypeConverter InnerConvertor => TypeDescriptor.GetConverter(ParentType.GenericTypeArguments[0]);
}

[JsonConverter(typeof(WrapperSerializer))]
[CustomModConfigItem(typeof(UI.WrapperElement))]
[TypeConverter("SpikysLib.Configs.UI.WrapperStringConverter")]
public class Wrapper {
    public Wrapper() => Value = default;
    public Wrapper(object? value) => Value = value;


    [JsonIgnore] public virtual PropertyFieldWrapper Member => new(GetType().GetProperty(GetType() == typeof(Wrapper) ? nameof(SaveBeforeEdit) : nameof(Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
    [JsonIgnore] public object? Value { get; set; }

    public Text SaveBeforeEdit { get; set; } = new();

    public Wrapper ChangeType(Type type) {
        Type genericType = typeof(Wrapper<>).MakeGenericType(type);
        if (GetType() == genericType) return this;
        return (Wrapper)Activator.CreateInstance(genericType, Value switch {
            JObject { Count: 0 } or JValue { Value: null } => Activator.CreateInstance(type),
            JToken token => token.ToObject(type),
            _ => Convert.ChangeType(Value, type),
        })!;
    }


    public override bool Equals(object? obj) => obj is Wrapper other && Value is not null && Value.Equals(other.Value);
    public override int GetHashCode() => Value!.GetHashCode();
    public override string? ToString() => Value?.ToString();

    public static Wrapper From(Type type) => (Wrapper)Activator.CreateInstance(typeof(Wrapper<>).MakeGenericType(type))!;
    public static Wrapper From(object value) => (Wrapper)Activator.CreateInstance(typeof(Wrapper<>).MakeGenericType(value.GetType()), value)!;
}

public class Wrapper<T> : Wrapper where T : new() {
    public Wrapper() => Value = new();
    public Wrapper(T value) => Value = value;

    [JsonIgnore] new public T Value { get => (T)base.Value!; set => base.Value = value; }

    public static implicit operator T(Wrapper<T> wrapper) => wrapper.Value;
}


public sealed class WrapperElement : ConfigElement<Wrapper> {

    public override void OnBind() {
        base.OnBind();
        Wrapper wrapper = Value;

        int top = 0;
        PropertyFieldWrapper member = wrapper.Member;
        (UIElement container, UIElement element) = ConfigManager.WrapIt(this, ref top, member, wrapper, 0);
        _element = (ConfigElement)element;
        container.Left.Pixels -= 20;
        container.Width.Pixels += 20;

        Reflection.ConfigElement.backgroundColor.SetValue(_element, Color.Transparent);
        Func<string> childText = Reflection.ConfigElement.TextDisplayFunction.GetValue(_element)!;
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_element, wrapper.Member.Name == nameof(Wrapper.Value) ? () => $"{TextDisplayFunction()}{childText()[member.Name.Length..]}" : () => $"{TextDisplayFunction()} ({childText()})");
        Reflection.ConfigElement.TooltipFunction.SetValue(_element, TooltipFunction);
        DrawLabel = false;
        TooltipFunction = null;
        MaxHeight.Pixels = int.MaxValue;
        Recalculate();
    }

    public override void Recalculate() {
        base.Recalculate();
        Height = _element.Height;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    private ConfigElement _element = null!;
}
