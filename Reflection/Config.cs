using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using TElement = Terraria.ModLoader.Config.UI.ConfigElement;
using TManager = Terraria.ModLoader.Config.ConfigManager;

namespace SpikysLib.Reflection;

public static class ConfigManager {
    public static readonly StaticMethod<object?> Save = new(typeof(TManager), nameof(Save), typeof(ModConfig));
    public static readonly StaticMethod<object?> Load = new(typeof(TManager), nameof(Load), typeof(ModConfig));
    public static readonly StaticMethod<string> GetLocalizedLabel = new(typeof(TManager), nameof(GetLocalizedLabel), typeof(PropertyFieldWrapper));
    public static readonly StaticMethod<string> GetLocalizedTooltip = new(typeof(TManager), nameof(GetLocalizedTooltip), typeof(PropertyFieldWrapper));
}

public static class ConfigElement {
    public static readonly Property<TElement, Func<string>> TextDisplayFunction = new(nameof(TextDisplayFunction));
    public static readonly Property<TElement, Func<string>> TooltipFunction = new(nameof(TooltipFunction));
    public static readonly Property<TElement, bool> DrawLabel = new(nameof(DrawLabel));
    public static readonly Field<TElement, Color> backgroundColor = new(nameof(backgroundColor));
    public static readonly Method<TElement, object?> DrawSelf = new(nameof(DrawSelf), typeof(SpriteBatch));
}

public static class UIModConfig {
    public static readonly Type Type = Main.tModLoader.GetType("Terraria.ModLoader.Config.UI.UIModConfig")!;
    public static readonly StaticProperty<string> Tooltip = new(Type, nameof(Tooltip));
}

public static class ObjectElement {
    public static readonly Type Type = Main.tModLoader.GetType("Terraria.ModLoader.Config.UI.ObjectElement")!;
    public static readonly FieldInfo pendingChanges = Type.GetField(nameof(pendingChanges), Member<FieldInfo>.InstanceFlags)!;
    public static readonly FieldInfo expandButton = Type.GetField(nameof(expandButton), Member<FieldInfo>.InstanceFlags)!;
    public static readonly FieldInfo expanded = Type.GetField(nameof(expanded), Member<FieldInfo>.InstanceFlags)!;
}