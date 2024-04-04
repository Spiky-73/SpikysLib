using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using SpikysLib.Extensions;
using SpikysLib.Configs;

namespace SpikysLib.IO;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ChoiceAttribute : Attribute { }

public sealed class MultiChoiceConverter : JsonConverter<MultiChoice> {
    public override MultiChoice ReadJson(JsonReader reader, Type objectType, [AllowNull] MultiChoice existingValue, bool hasExistingValue, JsonSerializer serializer) {
        existingValue ??= (MultiChoice)Activator.CreateInstance(objectType)!;
        if (objectType.IsSubclassOfGeneric(typeof(MultiChoice<>), out Type? type)) {
            existingValue.Data = serializer.Deserialize(reader, type.GenericTypeArguments[0]);
        } else {
            JObject jObject = serializer.Deserialize<JObject>(reader)!;
            JProperty property = (JProperty)jObject.First!;
            existingValue.Choice = property.Name;
            existingValue.Data = property.Value.ToObject(existingValue.Choices[existingValue.ChoiceIndex].Type);
        }
        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, [AllowNull] MultiChoice value, JsonSerializer serializer) {
        if (value is null) return;
        if (value.GetType().IsSubclassOfGeneric(typeof(MultiChoice<>), out _)) {
            serializer.Serialize(writer, value?.Data);
        } else {
            writer.WriteStartObject();
            writer.WritePropertyName(value.Choice);
            serializer.Serialize(writer, value.Data);
            writer.WriteEndObject();
        }
    }
}
