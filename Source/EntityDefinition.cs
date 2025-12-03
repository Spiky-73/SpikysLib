using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpikysLib.Configs.UI;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SpikysLib;

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

public abstract class EntityDefinition<TDefinition, TEntity> : EntityDefinition<TDefinition>, IEntityDefinition where TDefinition : EntityDefinition<TDefinition, TEntity> where TEntity: notnull, ILocalizedModType {
    public EntityDefinition() : base() { }
    public EntityDefinition(string key) : base(key) { }
    public EntityDefinition(string mod, string name) : base(mod, name) { }
    public EntityDefinition(TEntity entity) : base(entity.Mod.Name, entity.Name) { }

    [JsonIgnore] public abstract TEntity? Entity { get; }
    public override int Type => Entity is null ? -1 : 1;

    [JsonIgnore] public override string DisplayName => Entity?.GetLocalizedValue("DisplayName") ?? base.DisplayName;
    [JsonIgnore] public override string? Tooltip => Entity?.GetLocalizedValue("Tooltip");
}