using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib;

public static class TypeHelper {

    public static bool ImplementsInterface(this Type type, Type iType, [NotNullWhen(true)] out Type? impl) => (impl = Array.Find(type.GetInterfaces(), i => iType.IsGenericType ? i.IsGenericType && i.GetGenericTypeDefinition() == iType : iType == i)) != null;

    public static bool IsSubclassOfGeneric(this Type type, Type generic, [NotNullWhen(true)] out Type? impl) {
        Type? t = type;
        while (t != null && t != typeof(object)) {
            Type cur = t.IsGenericType ? t.GetGenericTypeDefinition() : t;
            if (generic == cur) {
                impl = t;
                return true;
            }
            t = t.BaseType;
        }
        impl = null;
        return false;
    }

    public static object? Retrieve(this object self, string name) => Retrieve(self.GetType(), name, null);
    public static object? Retrieve(this Type type, string name) => Retrieve(type, name, null);
    private static object? Retrieve(Type type, string name, object? self = null) => type.InvokeMember(name, BindingFlags.GetField | BindingFlags.GetProperty, null, self, []);

    public static void Assign(this object self, string name, object value) => Assign(self.GetType(), name, value, self);
    public static void Assign(this Type type, string name, object value) => Assign(type, name, null, value);
    private static void Assign(Type type, string name, object? self, object value) => type.InvokeMember(name, BindingFlags.SetField | BindingFlags.SetProperty, null, self, [value]);

    public static object? Call(this object self, string name, params object[] args) => Call(self.GetType(), name, self, args);
    public static object? Call(this Type type, string name, params object[] args) => Call(type, name, null, args);
    private static object? Call(Type type, string name, object? self, params object[] args) => type.InvokeMember(name, BindingFlags.InvokeMethod, null, self, args);

    public static PropertyFieldWrapper[] GetPropertiesFields(this Type type, BindingFlags flags = BindingFlags.Default) {
        PropertyInfo[] properties = type.GetProperties(flags);
        FieldInfo[] fields = type.GetFields(flags);
        return fields.Select(x => new PropertyFieldWrapper(x)).Concat(properties.Select(x => new PropertyFieldWrapper(x))).ToArray();
    }

    public static (PropertyFieldWrapper member, object? value) GetPropertyFieldValue(this Type host, object? obj, params string[] members) {
        PropertyFieldWrapper member = host.GetPropertyField(members[0], AnyMemberFlags | BindingFlags.FlattenHierarchy) ?? throw new MissingMemberException(host.Name, members[0]);
        return members.Length == 1 ? (member, member.GetValue(obj)) : member.Type.GetPropertyFieldValue(member.GetValue(obj), members[1..]);
    }

    public static PropertyFieldWrapper? GetPropertyField(this Type type, string name, BindingFlags flags = BindingFlags.Default) {
        FieldInfo? field = type.GetField(name, flags);
        if (field is not null) return new(field);
        PropertyInfo? prop = type.GetProperty(name, flags);
        if (prop is not null) return new(prop);
        return null;
    }

    public const BindingFlags AnyMemberFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}