using System;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Reflection;

[Obsolete($"use {nameof(Extensions)}.{nameof(Extensions.TypeExtensions)} instead", true)]
public static class ReflectionHelper {
    public static object Retrieve(this object self, string name) =>  Extensions.TypeExtensions.Retrieve(self, name);
    public static object Retrieve(this Type type, string name, object? self = null) =>  Extensions.TypeExtensions.Retrieve(type, name);
    public static void Assign(this object self, string name, object value) =>  Extensions.TypeExtensions.Assign(self, name, value);
    public static void Assign(this Type type, string name, object value, object? self = null) =>  Extensions.TypeExtensions.Assign(type, name, value);
    public static object? Call(this object self, string name, params object[] args) =>  Extensions.TypeExtensions.Call(self, name, args);
    public static object? Call(this Type type, string name, params object[] args) =>  Extensions.TypeExtensions.Call(type, name, args);
    public static PropertyFieldWrapper? GetPropertyField(this Type type, string name, BindingFlags flags = BindingFlags.Default) => Extensions.TypeExtensions.GetPropertyField(type, name, flags);
    public const BindingFlags Flags = Extensions.TypeExtensions.ReflectionFlags;
}