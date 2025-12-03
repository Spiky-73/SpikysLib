using System;
using System.ComponentModel;
using System.Reflection;

namespace SpikysLib.IO;

public class ToFromStringConverter : TypeConverter {
    public ToFromStringConverter(Type type) {
        Console.WriteLine(type.AssemblyQualifiedName);
        MethodInfo? fromString = type.GetMethod("FromString", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, [typeof(string)]);
        if (fromString is null || fromString.ReturnType != type) throw new ArgumentException($"The type {type} does not have a public static FromString(string) method that returns a {type}");
        FromString = fromString;
    }

    public sealed override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType != typeof(string) && base.CanConvertTo(context, destinationType);
    public sealed override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    public sealed override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value) => value is string ? FromString.Invoke(null, [value]) : base.ConvertFrom(context, culture, value);
    public MethodInfo FromString { get; }
}