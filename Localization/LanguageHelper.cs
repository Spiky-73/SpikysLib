using System;
using System.Linq;
using Terraria.Localization;

namespace SpikysLib.Localization;

public delegate string KeyModifier(string key); 

public static class LanguageHelper {
    public static void RegisterLocalizationKeysForMembers(Type type) => Reflection.ConfigManager.RegisterLocalizationKeysForMembers.Invoke(type);

    public static event KeyModifier? ModifyKey;

    internal static void Load() {
        On_LanguageManager.GetText += HookGetText;
        On_LanguageManager.GetOrRegister += HookGetOrRegister;
        On_LanguageManager.GetTextValue_string += HookGetTextValue;
        On_LanguageManager.GetTextValue_string_object += HookGetTextValue;
        On_LanguageManager.GetTextValue_string_object_object += HookGetTextValue;
        On_LanguageManager.GetTextValue_string_object_object_object += HookGetTextValue;
        On_LanguageManager.GetTextValue_string_ObjectArray += HookGetTextValue;
        On_LanguageManager.Exists += HookExists;
    }
    internal static void Unload() {}

    private static LocalizedText HookGetText(On_LanguageManager.orig_GetText orig, LanguageManager self, string key) => orig(self, ChainModifiers(key));
    private static LocalizedText HookGetOrRegister(On_LanguageManager.orig_GetOrRegister orig, LanguageManager self, string key, Func<string> makeDefaultValue) => orig(self, ChainModifiers(key), makeDefaultValue);
    private static string HookGetTextValue(On_LanguageManager.orig_GetTextValue_string orig, LanguageManager self, string key) => orig(self, ChainModifiers(key));
    private static string HookGetTextValue(On_LanguageManager.orig_GetTextValue_string_object orig, LanguageManager self, string key, object arg0) => orig(self, ChainModifiers(key), arg0);
    private static string HookGetTextValue(On_LanguageManager.orig_GetTextValue_string_object_object orig, LanguageManager self, string key, object arg0, object arg1) => orig(self, ChainModifiers(key), arg0, arg1);
    private static string HookGetTextValue(On_LanguageManager.orig_GetTextValue_string_object_object_object orig, LanguageManager self, string key, object arg0, object arg1, object arg2) => orig(self, ChainModifiers(key), arg0, arg1, arg2);
    private static string HookGetTextValue(On_LanguageManager.orig_GetTextValue_string_ObjectArray orig, LanguageManager self, string key, object[] args) => orig(self, ChainModifiers(key), args);
    private static bool HookExists(On_LanguageManager.orig_Exists orig, LanguageManager self, string key) => orig(self, ChainModifiers(key));
    
    private static string ChainModifiers(string key) {
        if (ModifyKey is null) return key;
        foreach (KeyModifier modifier in ModifyKey.GetInvocationList().Cast<KeyModifier>()) key = modifier.Invoke(key);
        return key;
    }
}