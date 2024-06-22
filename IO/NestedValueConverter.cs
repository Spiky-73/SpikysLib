using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using SpikysLib.Extensions;
using SpikysLib.Configs;

namespace SpikysLib.IO;

public sealed class NestedValueConverter : JsonConverter<INestedValue> {

    public const string KeyProperty = ".key";

    public override INestedValue ReadJson(JsonReader reader, Type objectType, [AllowNull] INestedValue existingValue, bool hasExistingValue, JsonSerializer serializer) {
        bool raw = !objectType.IsSubclassOfGeneric(typeof(NestedValue<,>), out Type? type);
        JObject jObject = serializer.Deserialize<JObject>(reader)!;
        
        existingValue ??= (INestedValue)Activator.CreateInstance(objectType)!;
        if (jObject.ContainsKey("Parent") || jObject.ContainsKey("Value")) {
            if (jObject.TryGetValue("Parent", out JToken? parent)) existingValue.Key = raw ? parent : parent.ToObject(type!.GenericTypeArguments[0])!;
            if (jObject.TryGetValue("Value", out JToken? value)) existingValue.Value = raw ? value : value.ToObject(type!.GenericTypeArguments[1])!;
            ModConfigExtensions.SaveLoadingConfig = true;
        } else {
            if (jObject.TryGetValue(KeyProperty, out JToken? key) && jObject.Remove(KeyProperty)) existingValue.Key = raw ? key : key.ToObject(type!.GenericTypeArguments[0])!;
            existingValue.Value = raw ? jObject : jObject.ToObject(type!.GenericTypeArguments[1])!;
        }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, [AllowNull] INestedValue value, JsonSerializer serializer) {
        if (value is null) return;
        JObject jObject = new() { { KeyProperty, JToken.FromObject(value.Key, serializer) } };
        jObject.Merge(JObject.FromObject(value.Value, serializer));
        serializer.Serialize(writer, jObject);
    }
}
