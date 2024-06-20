using System;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Extensions;

public interface IPortableConfig {
    bool PortMembers();
}


public static class ModConfigExtensions {

    internal static void HookPort(Action<ModConfig> orig, ModConfig config) {
        orig(config);
        if (config is IPortableConfig pc && pc.PortMembers()) config.Save();
    }

    public static void Save(this ModConfig config) => Reflection.ConfigManager.Save.Invoke(config);
    public static void Load(this ModConfig config) => Reflection.ConfigManager.Load.Invoke(config);

    public static bool TryPortMember(this ModConfig config, string old, string cur) => config.TryPortMember(config, old, cur);
    public static bool TryPortMember(this ModConfig _, object self, string old, string cur) => TryPortMember(self, old, cur);
    public static bool TryPortMember(object self, string old, string cur) {
        (PropertyFieldWrapper oldMember, object? oldValue) = self.GetType().GetPropertyFieldValue(self, old);
        (PropertyFieldWrapper curMember, object? curValue) = self.GetType().GetPropertyFieldValue(self, cur);
        if (Equals(oldValue, curValue)) return false;
        curMember.SetValue(self, oldValue);
        oldMember.SetValue(self, oldMember.MemberInfo.GetCustomAttribute<DefaultValueAttribute>()?.Value ?? (oldMember.Type.IsValueType ? Activator.CreateInstance(oldMember.Type) : null));
        return true;
    }
    public static bool TryPortMember<T>(this ModConfig _, ref T old, ref T cur, T def) => TryPortMember(ref old, ref cur, def);
    public static bool TryPortMember<T>(ref T old, ref T cur, T def) {
        if (Equals(old, def)) return false;
        (cur, old) = (old, def);
        return true;
    }

    public static void SetInstance(object instance, bool unload = false) => instance.GetType().GetField("Instance", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)?.SetValue(null, unload ? null : instance);
}