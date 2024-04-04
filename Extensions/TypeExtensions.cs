using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Terraria.ModLoader.Config.UI;

namespace SpikysLib.Extensions;

public static class TypeExtensions {

    public static bool ImplementsInterface(this Type type, Type iType, [NotNullWhen(true)] out Type? impl)
        => (impl = Array.Find(type.GetInterfaces(), i => iType.IsGenericType ? i.IsGenericType && i.GetGenericTypeDefinition() == iType : iType == i)) != null;
    
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

    public static (PropertyFieldWrapper, object?) GetMember(this Type host, object? obj, params string[] members) {
        MemberInfo member = host.GetMember(members[0], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)[0];
        if (member is PropertyInfo property) {
            if (members.Length == 1) return (new(property), obj);
            obj = property.GetValue(obj);
            host = property.PropertyType;
        } else if (member is FieldInfo field) {
            if (members.Length == 1) return (new(field), obj);
            obj = field.GetValue(obj);
            host = field.FieldType;
        }
        return host.GetMember(obj, members[1..]);
    }
}