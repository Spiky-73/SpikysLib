using System;
using System.Reflection;
using Terraria.ModLoader.Config;

namespace SpikysLib.Extensions;

public interface IPortableConfig {
    bool PortMembers();
}


public static class ModConfigExtensions {

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    internal static void HookPort(Action<ModConfig> orig, ModConfig config) {
        orig(config);
        if (config is IPortableConfig pc && pc.PortMembers()) config.Save();
    }

    public static bool TryPortMember<T>(this ModConfig _, ref T old, ref T cur, T def) => TryPortMember(ref old, ref cur, def);
    public static bool TryPortMember<T>(ref T old, ref T cur, T def) {
        if (Equals(old, def)) return false;
        (cur, old) = (old, def);
        return true;
    }

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);
}