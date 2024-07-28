using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SpikysLib.Configs;

public static class ConfigHelper {

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);

    public static void MoveMember<TConfig>(bool cond, Action<TConfig> move) where TConfig: ModConfig => MoveMember(cond, c => move((TConfig)c));
    public static void MoveMember(bool cond, Action<ModConfig> move) {
        if (cond && s_loading) _moves.Add(move);
    }
    public static bool SaveLoadingConfig() => s_saveLoading = true;

    private static void HookLoad(Action<ModConfig> orig, ModConfig config) {
        (s_loading, s_saveLoading) = (true, false);
        _moves.Clear();

        orig(config);
        s_loading = false;

        if (_moves.Count != 0) {
            foreach (var move in _moves) move(config);
            _moves.Clear();
            SaveLoadingConfig();
        }
        if (s_saveLoading) config.Save();
        s_saveLoading = false;
    }

    private static bool s_loading;
    private static bool s_saveLoading;
    private static readonly List<Action<ModConfig>> _moves = [];

    internal static void Load() => MonoModHooks.Add(Reflection.ConfigManager.Load, HookLoad);
    internal static void Unload() { }
}