using System;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Reflection;

public static class ReflectionHelper {
    public static object Retrieve(this object self, string name) => self.GetType().Retrieve(name, self);
    public static object Retrieve(this Type type, string name, object? self = null) => GetPropertyField(type, name).GetValue(self);

    public static void Assign(this object self, string name, object value) => Assign(self.GetType(), name, value, self);
    public static void Assign(this Type type, string name, object value, object? self = null) => GetPropertyField(type, name).SetValue(self, value);

    public static object? Call(this object self, string name, params object[] args) => CallFull(self.GetType(), name, self, args);
    public static object? Call(this Type type, string name, params object[] args) => CallFull(type, name, null, args);
    public static object? CallFull(this Type type, string name, object? self, params object[] args) {
        Type[] types = new Type[args.Length];
        for (int i = 0; i < args.Length; i++) types[i] = args[i].GetType();
        return GetMethod(type, name, type).Invoke(self, args);
    }

    // TODO cache results
    public static PropertyFieldWrapper GetPropertyField(this Type type, string name) {
        FieldInfo? field = type.GetField(name, Flags);
        if (field is not null) return new(field);
        return new(type.GetProperty(name, Flags) ?? throw new MissingMemberException(type.FullName, name));
    }
    public static FieldInfo GetField(this Type type, string name) {
        return type.GetField(name, Flags) ?? throw new MissingFieldException(type.FullName, name);
    }
    public static PropertyInfo GetProperty(this Type type, string name) {
        return type.GetProperty(name, Flags) ?? throw new MissingMemberException(type.FullName, name);
    }
    public static MethodInfo GetMethod(this Type type, string name, params Type[] argTypes) {
        return type.GetMethod(name, Flags, argTypes) ?? throw new MissingMethodException(type.FullName, name); ;
    }

    public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}