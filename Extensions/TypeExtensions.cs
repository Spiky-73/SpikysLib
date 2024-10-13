using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(TypeHelper)} instead", true)]
public static class TypeExtensions {

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.ImplementsInterface)} instead", true)]
    public static bool ImplementsInterface(this Type type, Type iType, [NotNullWhen(true)] out Type? impl) => TypeHelper.ImplementsInterface(type, iType, out impl);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.IsSubclassOfGeneric)} instead", true)]
    public static bool IsSubclassOfGeneric(this Type? type, Type generic, [NotNullWhen(true)] out Type? impl) => TypeHelper.ImplementsInterface(type!, generic, out impl);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Retrieve)} instead", true)]
    public static object? Retrieve(this object self, string name) => TypeHelper.Retrieve(self, name);
    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Retrieve)} instead", true)]
    public static object? Retrieve(this Type type, string name) => TypeHelper.Retrieve(type, name);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Assign)} instead", true)]
    public static void Assign(this object self, string name, object value) => TypeHelper.Assign(self, name, value);
    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Assign)} instead", true)]
    public static void Assign(this Type type, string name, object value) => TypeHelper.Assign(type, name, value);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Call)} instead", true)]
    public static object? Call(this object self, string name, params object[] args) => TypeHelper.Call(self, name, args);
    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.Call)} instead", true)]
    public static object? Call(this Type type, string name, params object[] args) => TypeHelper.Call(type, name, args);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.GetPropertyFieldValue)} instead", true)]
    public static (PropertyFieldWrapper member, object? value) GetPropertyFieldValue(this Type host, object? obj, params string[] members) => TypeHelper.GetPropertyFieldValue(host, obj, members);

    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.GetPropertiesFields)} instead", true)]
    public static PropertyFieldWrapper[] GetPropertiesFields(this Type type, BindingFlags flags = BindingFlags.Default) => TypeHelper.GetPropertiesFields(type, flags);
    [Obsolete($"use {nameof(TypeHelper)}.{nameof(TypeHelper.GetPropertyField)} instead", true)]
    public static PropertyFieldWrapper? GetPropertyField(this Type type, string name, BindingFlags flags = BindingFlags.Default) => TypeHelper.GetPropertyField(type, name, flags);

    public const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}