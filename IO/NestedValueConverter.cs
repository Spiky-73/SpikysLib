using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using SpikysLib.Extensions;
using SpikysLib.Configs;

namespace SpikysLib.IO;

public sealed class NestedValueConverter : JsonConverter<INestedValue> {

    public const string KeyProperty = "key";
    public const string ValueProperty = "value";

    public override INestedValue ReadJson(JsonReader reader, Type objectType, [AllowNull] INestedValue existingValue, bool hasExistingValue, JsonSerializer serializer) {
        bool raw = !objectType.IsSubclassOfGeneric(typeof(NestedValue<,>), out Type? type);
        JObject obj = serializer.Deserialize<JObject>(reader)!;
        existingValue ??= (INestedValue)Activator.CreateInstance(objectType)!;
        
        // Compatibility version < v1.1
        if (obj.Count <= 2 && obj.ContainsKey("Parent") || obj.ContainsKey("Value")) {
            if (obj.TryGetValue("Parent", out JToken? parent)) existingValue.Key = raw ? parent : parent.ToObject(type!.GenericTypeArguments[0])!;
            if (obj.TryGetValue("Value", out JToken? value)) existingValue.Value = raw ? value : value.ToObject(type!.GenericTypeArguments[1])!;
            PortConfig.SaveLoadingConfig = true;
            return existingValue;
        }
        if (NeedsLegacyMode(obj)) {
            if (obj.TryGetValue(KeyProperty, out JToken? key)) existingValue.Key = raw ? key : key.ToObject(type!.GenericTypeArguments[0])!;
            if (obj.TryGetValue(ValueProperty, out JToken? value)) existingValue.Value = raw ? value : value.ToObject(type!.GenericTypeArguments[1])!;
        } else {
            if (obj.TryGetValue($".{KeyProperty}", out JToken? key) && obj.Remove($".{KeyProperty}")) existingValue.Key = raw ? key : key.ToObject(type!.GenericTypeArguments[0])!;
            existingValue.Value = raw ? obj : obj.ToObject(type!.GenericTypeArguments[1])!;
        }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, [AllowNull] INestedValue value, JsonSerializer serializer) {
        if (value is null) return;
        JObject obj;
        JObject val = JObject.FromObject(value.Value, serializer);
        if (val.ContainsKey($".{KeyProperty}") || NeedsLegacyMode(val)) {
            obj = new() {
                { KeyProperty, JToken.FromObject(value.Key, serializer) },
                { ValueProperty, JObject.FromObject(value.Value, serializer) }
            };
        } else {
            obj = new() { { $".{KeyProperty}", JToken.FromObject(value.Key, serializer) } };
            obj.Merge(JObject.FromObject(value.Value, serializer));
        }
        serializer.Serialize(writer, obj);
    }

    private static bool NeedsLegacyMode(JObject obj) => obj.Count switch {
        1 => obj.ContainsKey(KeyProperty) || obj.ContainsKey(ValueProperty),
        2 => obj.ContainsKey(KeyProperty) && obj.ContainsKey(ValueProperty),
        _ => false
    };
}
