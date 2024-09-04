using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        if ((obj.Count == 1 && (obj.ContainsKey("Parent") || obj.ContainsKey("Value")))
        || (obj.Count == 2 && obj.ContainsKey("Parent") && obj.ContainsKey("Value"))) {
            if (obj.TryGetValue("Parent", out JToken? parent)) existingValue.Key = raw ? parent : parent.ToObject(type!.GenericTypeArguments[0])!;
            if (obj.TryGetValue("Value", out JToken? value)) existingValue.Value = raw ? value : value.ToObject(type!.GenericTypeArguments[1])!;
            ConfigHelper.SaveLoadingConfig();
            return existingValue;
        }
        if (!IsUpdated(obj)) {
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
        JToken? key = value.Key is not null ? JToken.FromObject(value.Key, serializer) : null;
        JToken? val = value.Value is not null ? JToken.FromObject(value.Value, serializer) : null;
        if (val is null || val is not JObject jobj || IsUpdated(jobj)) {
            obj = new() {
                { KeyProperty, key },
                { ValueProperty, val }
            };
        } else {
            obj = new() { { $".{KeyProperty}", key } };
            obj.Merge(val);
        }
        serializer.Serialize(writer, obj);
    }

    private static bool IsUpdated(JObject obj) => obj.ContainsKey($".{KeyProperty}");
}
