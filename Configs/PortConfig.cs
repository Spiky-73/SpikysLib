using System;
using System.Collections.Generic;
using SpikysLib.Extensions;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SpikysLib;

public static class PortConfig {
    public static void MoveMember<TConfig>(bool cond, Action<TConfig> move) where TConfig: ModConfig => MoveMember(cond, c => move((TConfig)c));
    public static void MoveMember(bool cond, Action<ModConfig> move) {
        if (cond && LoadingConfig) _moves.Add(move);
    }

    private static void HookPort(Action<ModConfig> orig, ModConfig config) {
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

    internal static void Load() => MonoModHooks.Add(Reflection.ConfigManager.Load, HookPort);
    internal static void Unload() { }
}