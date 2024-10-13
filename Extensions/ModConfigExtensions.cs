using System;
using SpikysLib.Configs;
using Terraria.ModLoader.Config;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(Configs)}.{nameof(ConfigHelper)} instead", true)]
public static class ModConfigExtensions {

    [Obsolete($"use {nameof(Configs)}.{nameof(ConfigHelper)}.{nameof(ConfigHelper.Save)} instead", true)]
    public static void Save(this ModConfig config) => ConfigHelper.Save(config);

    [Obsolete($"use {nameof(Configs)}.{nameof(ConfigHelper)}.{nameof(ConfigHelper.Load)} instead", true)]
    public static void Load(this ModConfig config) => ConfigHelper.Load(config);

    public static void SetInstance(object instance, bool unload = false) => ConfigHelper.SetInstance(instance, unload);
}