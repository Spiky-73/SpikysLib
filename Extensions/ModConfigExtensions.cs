using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace SpikysLib.Extensions;

public static class ModConfigExtensions {

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);
    
    public static void MoveMember<TConfig>(bool cond, Action<TConfig> move) where TConfig: ModConfig => MoveMember(cond, c => move((TConfig)c));
    public static void MoveMember(bool cond, Action<ModConfig> move) {
        if (cond && LoadingConfig) _moves.Add(move);
    }

    internal static void HookPort(Action<ModConfig> orig, ModConfig config) {
        LoadingConfig = true;
        SaveLoadingConfig = false;
        _moves.Clear();
        orig(config);
        LoadingConfig = false;

        if (_moves.Count != 0) {
            foreach (var m in _moves) m(config);
            _moves.Clear();
            SaveLoadingConfig = true;
        }
        if (SaveLoadingConfig) config.Save();
        SaveLoadingConfig = false;
    }

    internal static bool LoadingConfig { get; private set; }
    public static bool SaveLoadingConfig { get; set; }
    private static readonly List<Action<ModConfig>> _moves = [];
}