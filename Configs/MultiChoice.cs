using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.ModLoader.Config.UI;
using System.Collections.ObjectModel;
using System;

namespace SpikysLib.Configs;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ChoiceAttribute : Attribute { }

[JsonConverter(typeof(IO.MultiChoiceConverter))]
[CustomModConfigItem(typeof(UI.MultiChoiceElement))]
public abstract class MultiChoice {

    [JsonIgnore] public ReadOnlyCollection<PropertyFieldWrapper> Choices { get; }
    [JsonIgnore] public int ChoiceIndex {
        get => _index;
        set => _index = (value + Choices.Count) % Choices.Count;
    }
    [JsonIgnore] public string Choice {
        get => Choices[ChoiceIndex].Name;
        set {
            for (int i = 0; i < Choices.Count; i++) {
                if (Choices[i].Name != value) continue;
                ChoiceIndex = i;
                return;
            }
        }
    }

    public bool TryGet<T>(string name, [MaybeNullWhen(false)] out T? value) {
        if (Choice == name) {
            value = (T?)Data;
            return true;
        }
        value = default;
        return false;
    }

    internal virtual object? Data {
        get => Choices[ChoiceIndex].GetValue(this);
        set => Choices[ChoiceIndex].SetValue(this, value);
    }

    public MultiChoice() {
        Choices = new([
            .. GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.GetCustomAttribute<ChoiceAttribute>() != null).Select(p => new PropertyFieldWrapper(p)),
            .. GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(field => field.GetCustomAttribute<ChoiceAttribute>() != null).Select(f => new PropertyFieldWrapper(f)),
        ]);
    }

    private int _index;
}

public abstract class MultiChoice<T> : MultiChoice {
    public MultiChoice() : base() { }
    public MultiChoice(T value) : base() => Value = value;
    internal override object? Data { get => Value; set => Value = (T?)value; }
    [JsonIgnore] public abstract T? Value { get; set; }

    public static implicit operator T?(MultiChoice<T> value) => value.Value;
}
