using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace SpikysLib.Configs.UI;

public interface IEntityDefinition {
    bool IsUnloaded { get; }
    int Type { get; }
    bool AllowNull { get; }
    string DisplayName { get; }
    string? Tooltip { get; }
    IList<IEntityDefinition> GetValues();
}

[CustomModConfigItem(typeof(EntityDefinitionElement))]
public abstract class EntityDefinition<TDefinition> : EntityDefinition, IEntityDefinition where TDefinition : EntityDefinition<TDefinition> {
    public EntityDefinition() : base() { }
    public EntityDefinition(string key) : base(key) { }
    public EntityDefinition(string mod, string name) : base(mod, name) { }

    [JsonIgnore] public override string DisplayName => $"{Name} [{Mod}]{(IsUnloaded ? $" ({Language.GetTextValue("Mods.ModLoader.Unloaded")})" : string.Empty)}";
    [JsonIgnore] public virtual string? Tooltip => null;

    [JsonIgnore] public virtual bool AllowNull => false;
    public abstract TDefinition[] GetValues();
    IList<IEntityDefinition> IEntityDefinition.GetValues() => GetValues();

    public static TDefinition FromString(string s) => (TDefinition)Activator.CreateInstance(typeof(TDefinition), s)!;
}
