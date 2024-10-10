using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpikysLib.Configs;

namespace SpikysLib.IO;

public sealed class WrapperConverter : JsonConverter<Wrapper> {
    public override Wrapper ReadJson(JsonReader reader, Type objectType, Wrapper? existingValue, bool hasExistingValue, JsonSerializer serializer) {
        existingValue ??= (Wrapper)Activator.CreateInstance(objectType)!;
        existingValue.Value = serializer.Deserialize(reader, objectType == typeof(Wrapper) ? typeof(JToken) : existingValue.Member.Type)!;
        return existingValue;
    }
    public override void WriteJson(JsonWriter writer, [AllowNull] Wrapper value, JsonSerializer serializer) => serializer.Serialize(writer, value?.Value);
}

public sealed class WrapperStringConverter : TypeConverter {
    public WrapperStringConverter(Type type) => ParentType = type;
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
        TypeConverter? converter = InnerConvertor();
        return converter is not null ? converter.CanConvertTo(context, destinationType) : base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
        TypeConverter? converter = InnerConvertor();
        return converter is not null ? converter.ConvertTo(context, culture, ((Wrapper?)value)?.Value, destinationType) : base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
        TypeConverter? converter = InnerConvertor();
        return converter is not null ? converter.CanConvertFrom(context, sourceType) : base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
        TypeConverter? converter = InnerConvertor();
        return converter is not null ? Activator.CreateInstance(ParentType, converter.ConvertFrom(context, culture, value)) : base.ConvertFrom(context, culture, value);
    }

    public Type ParentType { get; }
    public TypeConverter? InnerConvertor() => ParentType.IsSubclassOfGeneric(typeof(Wrapper<>), out Type? impl) ? TypeDescriptor.GetConverter(impl.GenericTypeArguments[0]) : null;
}
