using System.Reflection;
using Terraria.ModLoader.Config;

namespace SpikysLib.Extensions;

public static class ModConfigExtensions {

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);
}