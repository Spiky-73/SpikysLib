using System;
using SpikysLib.Configs;
using Terraria.ModLoader.Config;

namespace SpikysLib;

[Obsolete($"use {nameof(Configs)}.{nameof(ConfigHelper)} instead", true)]
public static class PortConfig {

    public static void MoveMember<TConfig>(bool cond, Action<TConfig> move) where TConfig: ModConfig => MoveMember(cond, c => move((TConfig)c));
    public static void MoveMember(bool cond, Action<ModConfig> move) => ConfigHelper.MoveMember(cond, move);

    public static bool SaveLoadingConfig { get => false; set => ConfigHelper.SaveLoadingConfig(); }
}