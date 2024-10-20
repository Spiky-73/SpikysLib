using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Configs;

public static class ConfigHelper {

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    public static IEnumerable<PropertyFieldWrapper> GetFieldsAndProperties(object item)
        => ConfigManager.GetFieldsAndProperties(item).Where(v => !Attribute.IsDefined(v.MemberInfo, typeof(JsonIgnoreAttribute)) || Attribute.IsDefined(v.MemberInfo, typeof(ShowDespiteJsonIgnoreAttribute)));

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);

    public static void MoveMember<TConfig>(bool cond, Action<TConfig> move) where TConfig: ModConfig => MoveMember(cond, c => move((TConfig)c));
    public static void MoveMember(bool cond, Action<ModConfig> move) {
        if (cond && s_loading) _moves.Add(move);
    }

    public static void ApplyMoves(ModConfig config) {
        if (_moves.Count == 0) return;
        foreach (var move in _moves) move(config);
        _moves.Clear();
        SaveLoadingConfig();
    }
    public static bool SaveLoadingConfig() => s_saveLoading = true;

    private static void HookLoad(Action<ModConfig> orig, ModConfig config) {
        (s_loading, s_saveLoading) = (true, false);
        _moves.Clear();

        orig(config);
        s_loading = false;
        ApplyMoves(config);

        if (s_saveLoading) config.Save();
        s_saveLoading = false;
    }

    public static string JoinTooltips(params Func<string?>?[] tooltips) => string.Join('\n', tooltips.Select(t => t?.Invoke()).Where(t => !string.IsNullOrEmpty(t)));

    private static bool s_loading;
    private static bool s_saveLoading;
    private static readonly List<Action<ModConfig>> _moves = [];

    internal static void Load() => MonoModHooks.Add(Reflection.ConfigManager.Load, HookLoad);
    internal static void Unload() { }
}