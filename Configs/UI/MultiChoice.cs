using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Terraria.ModLoader.Config.UI;
using System.Collections.ObjectModel;
using Terraria.UI;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using SpikysLib.Extensions;

namespace SpikysLib.Configs.UI;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ChoiceAttribute : Attribute {} 

public sealed class MultiChoiceConverter : JsonConverter<MultiChoice> {
    public override MultiChoice ReadJson(JsonReader reader, Type objectType, [AllowNull] MultiChoice existingValue, bool hasExistingValue, JsonSerializer serializer) {
        existingValue ??= (MultiChoice)Activator.CreateInstance(objectType)!;
        if(objectType.IsSubclassOfGeneric(typeof(MultiChoice<>), out Type? type)) {
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
        if(value is null) return;
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

[JsonConverter(typeof(MultiChoiceConverter))]
[CustomModConfigItem(typeof(MultiChoiceElement))]
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

    public bool TryGet<T>(string name, [MaybeNullWhen(false)] out T? value){
        if(Choice == name) {
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

    public MultiChoice(){
        List<PropertyFieldWrapper> choices = new();
        choices.AddRange(GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.GetCustomAttribute<ChoiceAttribute>() != null).Select(p => new PropertyFieldWrapper(p)));
        choices.AddRange(GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(field => field.GetCustomAttribute<ChoiceAttribute>() != null).Select(f => new PropertyFieldWrapper(f)));
        Choices = choices.AsReadOnly();
    }

    private int _index;
}

public abstract class MultiChoice<T> : MultiChoice {
    public MultiChoice() : base() {}
    public MultiChoice(T value) : base() => Value = value;
    internal override object? Data { get => Value; set => Value = (T?)value; }
    [JsonIgnore] public abstract T? Value { get; set; }

    public static implicit operator T?(MultiChoice<T> value) => value.Value;
}


public sealed class MultiChoiceElement : ConfigElement<MultiChoice> {

    public override void OnBind() {
        base.OnBind();
        SetupMember();
    }

    public void SetupMember() {
        RemoveAllChildren();

        MultiChoice value = Value ??= (MultiChoice)Activator.CreateInstance(MemberInfo.Type)!;
        PropertyFieldWrapper selectedProp = value.Choices[value.ChoiceIndex];

        int top = 0;
        (UIElement container, UIElement element) = ConfigManager.WrapIt(this, ref top, selectedProp, value, 0);
        _selectedElement = (ConfigElement)element;
        container.Left.Pixels -= 20;
        container.Width.Pixels -= 7;

        _tooltip = TooltipFunction;
        DrawLabel = false;
        TooltipFunction = null;

        MaxHeight.Pixels = int.MaxValue;
        Reflection.ConfigElement.backgroundColor.SetValue(_selectedElement, Color.Transparent);

        Func<string> elementLabel = Reflection.ConfigElement.TextDisplayFunction.GetValue(_selectedElement)!;
        Func<string>? elementTooltip = Reflection.ConfigElement.TooltipFunction.GetValue(_selectedElement);
        Reflection.ConfigElement.TextDisplayFunction.SetValue(_selectedElement, () => $"{TextDisplayFunction()}: {elementLabel()}");
        Reflection.ConfigElement.TooltipFunction.SetValue(_selectedElement, () => {
            List<string> parts = new();
            string? p = null;
            if ((p = _tooltip?.Invoke()) is not null && p.Length != 0) parts.Add(p);
            if ((p = elementTooltip?.Invoke()) is not null && p.Length != 0) parts.Add(p);
            return string.Join('\n', parts);
        });

        int count = value.Choices.Count;
        UIImage swapButton;
        if (count == 2) {
            swapButton = new HoverImage(PlayTexture, Language.GetTextValue("Mods.SPikysLib.UI.Change", value.Choices[(value.ChoiceIndex + 1) % count].Name));
            swapButton.OnLeftClick += (UIMouseEvent a, UIElement b) => ChangeChoice(value.ChoiceIndex + 1);
        } else {
            swapButton = new HoverImageSplit(UpDownTexture, Language.GetTextValue("Mods.SPikysLib.UI.Change", value.Choices[(value.ChoiceIndex + 1) % count].Name), Language.GetTextValue("Mods.SPikysLib.UI.Change", value.Choices[(value.ChoiceIndex - 1 + count) % count].Name));
            swapButton.OnLeftClick += (UIMouseEvent a, UIElement b) => ChangeChoice(value.ChoiceIndex + (((HoverImageSplit)swapButton).HoveringUp ? 1 : -1));
        }
        swapButton.VAlign = 0.5f;
        swapButton.Left.Set(-30 + 5, 1);
        Append(swapButton);
        Recalculate();
    }

    public void ChangeChoice(int index) {
        Value.ChoiceIndex = index;
        SetupMember();
        ConfigManager.SetPendingChanges();
    }

    public override void Recalculate() {
        base.Recalculate();
        Height = _selectedElement.Height;
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(Height.Pixels, 0f);
    }

    private ConfigElement _selectedElement = null!;
    private Func<string>? _tooltip = null;

}