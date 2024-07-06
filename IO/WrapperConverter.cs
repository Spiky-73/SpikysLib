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
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => InnerConvertor.CanConvertTo(context, destinationType);
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) => InnerConvertor.ConvertTo(context, culture, ((Wrapper?)value)?.Value, destinationType);
    
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => InnerConvertor.CanConvertFrom(context, sourceType);
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => Activator.CreateInstance(ParentType, InnerConvertor.ConvertFrom(context, culture, value));

    public Type ParentType { get; }
    public TypeConverter InnerConvertor => TypeDescriptor.GetConverter(ParentType.GenericTypeArguments[0]);
}
