using System.Reflection;
using Terraria.ModLoader.Config;

namespace SpikysLib;

public static class ConfigHelper {

    public static void SaveConfig(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);

    public static void SetInstance<T>(T instance, bool unload = false) where T : notnull => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);

}