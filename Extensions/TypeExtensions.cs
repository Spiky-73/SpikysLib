using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Extensions;

public static class TypeExtensions {

    public static bool ImplementsInterface(this Type type, Type iType, [NotNullWhen(true)] out Type? impl) => (impl = Array.Find(type.GetInterfaces(), i => iType.IsGenericType ? i.IsGenericType && i.GetGenericTypeDefinition() == iType : iType == i)) != null;

    public static bool IsSubclassOfGeneric(this Type? type, Type generic, [NotNullWhen(true)] out Type? impl) {
        while (type != null && type != typeof(object)) {
            Type cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (generic == cur) {
                impl = type;
                return true;
            }
            type = type.BaseType;
        }
        impl = null;
        return false;
    }

    [Obsolete($"use {nameof(GetPropertyFieldValue)} instead", true)]
    public static (PropertyFieldWrapper, object?) GetMember(this Type host, object? obj, params string[] members) => host.GetPropertyFieldValue(obj, members);
    public static (PropertyFieldWrapper, object?) GetPropertyFieldValue(this Type host, object? obj, params string[] members) {
        PropertyFieldWrapper member = host.GetPropertyField(members[0], ReflectionFlags | BindingFlags.FlattenHierarchy) ?? throw new MissingMemberException(host.Name, members[0]);
        if (members.Length == 1) return (member, obj);
        obj = member.GetValue(obj);
        host = member.Type;
        return host.GetPropertyFieldValue(obj, members[1..]);
    }

    public static object Retrieve(this object self, string name) => Retrieve(self.GetType(), name, null);
    public static object Retrieve(this Type type, string name) => Retrieve(type, name, null);
    private static object Retrieve(Type type, string name, object? self = null) => (GetPropertyField(type, name, ReflectionFlags) ?? throw new MissingMemberException(type.Name, name)).GetValue(self);

    public static void Assign(this object self, string name, object value) => Assign(self.GetType(), name, value, self);
    public static void Assign(this Type type, string name, object value) => Assign(type, name, null, value);
    private static void Assign(Type type, string name, object? self, object value) => (GetPropertyField(type, name, ReflectionFlags) ?? throw new MissingMemberException(type.Name, name)).SetValue(self, value);

    public static object? Call(this object self, string name, params object[] args) => Call(self.GetType(), name, self, args);
    public static object? Call(this Type type, string name, params object[] args) => Call(type, name, null, args);
    private static object? Call(Type type, string name, object? self, params object[] args) {
        Type[] types = new Type[args.Length];
        for (int i = 0; i < args.Length; i++) types[i] = args[i].GetType();
        return (type.GetMethod(name, ReflectionFlags, types) ?? throw new MissingMethodException(type.Name, name)).Invoke(self, args);
    }

    public static PropertyFieldWrapper[] GetPropertiesFields(this Type type, BindingFlags flags = BindingFlags.Default) {
        PropertyInfo[] properties = type.GetProperties(flags);
        FieldInfo[] fields = type.GetFields(flags);
        return fields.Select(x => new PropertyFieldWrapper(x)).Concat(properties.Select(x => new PropertyFieldWrapper(x))).ToArray();
    }
    public static PropertyFieldWrapper? GetPropertyField(this Type type, string name, BindingFlags flags = BindingFlags.Default) {
        FieldInfo? field = type.GetField(name, flags);
        if (field is not null) return new(field);
        PropertyInfo? prop = type.GetProperty(name, flags);
        if (prop is not null) return new(prop);
        return null;
    }

    public const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}