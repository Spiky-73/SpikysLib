using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using SpikysLib.Extensions;
using SpikysLib.Configs;

namespace SpikysLib.IO;

public sealed class MultiChoiceConverter : JsonConverter<MultiChoice> {

    public const string ChoiceProperty = "choice";

    public override MultiChoice ReadJson(JsonReader reader, Type objectType, [AllowNull] MultiChoice existingValue, bool hasExistingValue, JsonSerializer serializer) {
        existingValue ??= (MultiChoice)Activator.CreateInstance(objectType)!;
        if (objectType.IsSubclassOfGeneric(typeof(MultiChoice<>), out Type? type)) {
            existingValue.Data = serializer.Deserialize(reader, type.GenericTypeArguments[0]);
        } else {
            JObject obj = serializer.Deserialize<JObject>(reader)!;
            if (NeedsLegacyMode(obj)) {
                JProperty property = (JProperty)obj.First!;
                existingValue.Choice = property.Name;
                existingValue.Data = property.Value.ToObject(existingValue.Choices[existingValue.ChoiceIndex].Type);
            } else {
                if (obj.TryGetValue($".{ChoiceProperty}", out JToken? choice) && obj.Remove($".{ChoiceProperty}")) existingValue.Choice = choice.ToObject<string>()!;
                existingValue.Data = obj.ToObject(existingValue.Choices[existingValue.ChoiceIndex].Type)!;
            }
        }
        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, [AllowNull] MultiChoice value, JsonSerializer serializer) {
        if (value is null) return;
        if (value.GetType().IsSubclassOfGeneric(typeof(MultiChoice<>), out _)) {
            serializer.Serialize(writer, value?.Data);
        } else {
            JObject obj;
            JObject? val = value.Data is null ? null : JObject.FromObject(value.Data, serializer);
            if (val is null || val.ContainsKey($".{ChoiceProperty}") || NeedsLegacyMode(val)) {
                obj = new() { { value.Choice, value.Data is null ? null : JToken.FromObject(value.Data, serializer) } };
            } else {
                obj = new() { { $".{ChoiceProperty}", JToken.FromObject(value.Choice, serializer) } };
                obj.Merge(JObject.FromObject(value.Data!, serializer));
            }
            serializer.Serialize(writer, obj);
        }
    }

    private static bool NeedsLegacyMode(JObject obj) => obj.Count == 1 && !obj.ContainsKey($".{ChoiceProperty}");
}
