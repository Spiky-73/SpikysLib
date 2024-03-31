using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib.Extensions;

public static class CollectionExtensions {

    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate) {
        for (int i = 0; i < list.Count; i++) if (predicate(list[i])) return i;
        return -1;
    }
    public static bool Exist<T>(this IEnumerable<T> collection, Predicate<T> predicate, [MaybeNullWhen(true)] out int count) {
        count = 0;
        foreach (T item in collection) {
            if (predicate(item)) return true;
            count++;
        }
        return false;
    }

    public static object Index(this ICollection collection, int index) {
        int i = 0;
        foreach (object o in collection) {
            if (i == index) return o;
            i++;
        }
        throw new IndexOutOfRangeException("The index was outside the bounds of the array");
    }

    public static bool TryAdd(this IOrderedDictionary dict, object key, object value) {
        if (dict.Contains(key)) return false;
        dict.Add(key, value);
        return true;
    }
    public static void Move(this IOrderedDictionary dict, int origIndex, int destIndex) => dict.Move(dict.Keys.Index(origIndex), destIndex);
    public static void Move(this IOrderedDictionary dict, object key, int destIndex) {
        object? value = dict[key];
        dict.Remove(key);
        dict.Insert(destIndex, key, value);
    }

    public static IEnumerable<(object key, object? value)> Items(this IDictionary dict) => dict.Items<object, object?>();
    public static IEnumerable<(Tkey key, TValue? value)> Items<Tkey, TValue>(this IDictionary dict) where Tkey : notnull {
        foreach (DictionaryEntry entry in dict) {
            yield return new((Tkey)entry.Key, (TValue?)entry.Value);
        }
    }
}
