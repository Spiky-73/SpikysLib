using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib.Collections;

public static class CollectionsHelper {
    public static object ItemAt(this IEnumerable enumerable, int index) {
        int i = 0;
        foreach (object o in enumerable) {
            if (i++ == index) return o;
        }
        throw new IndexOutOfRangeException("The index was outside the bounds of the array");
    }
    public static bool Exist<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) => enumerable.Exist(predicate, out _);
    public static bool Exist<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(true)] out int count) {
        count = 0;
        foreach (T item in enumerable) {
            if (predicate(item)) return true;
            count++;
        }
        return false;
    }

    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate) {
        for (int i = 0; i < list.Count; i++) if (predicate(list[i])) return i;
        return -1;
    }
    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate) {
        for (int i = 0; i < list.Count; i++) if (predicate(list[i])) return i;
        return -1;
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> builder) => dict.TryGetValue(key, out TValue? value) ? value : (dict[key] = builder());
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) => dict.GetOrAdd(key, () => value);
    public static TItem GetOrAddRaw<TKey, TValue, TItem>(this IDictionary<TKey, TValue> dict, TKey key, Func<TItem> builder) where TItem : TValue => (TItem)dict.GetOrAdd(key, () => builder())!;
    public static TItem GetOrAddRaw<TKey, TValue, TItem>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) where TItem : TValue => (TItem)dict.GetOrAddRaw(key, () => value)!;

    public static IEnumerable<(object key, object? value)> Items(this IDictionary dict) => dict.Items<object, object?>();
    public static IEnumerable<(TKey key, TValue value)> Items<TKey, TValue>(this IDictionary dict) where TKey : notnull {
        foreach (DictionaryEntry entry in dict) {
            yield return new((TKey)entry.Key, (TValue)entry.Value!);
        }
    }

    public static bool TryAdd(this IOrderedDictionary dict, object key, object value) {
        if (dict.Contains(key)) return false;
        dict.Add(key, value);
        return true;
    }
    public static void Move(this IOrderedDictionary dict, int origIndex, int destIndex) => dict.Move(dict.Keys.ItemAt(origIndex), destIndex);
    public static void Move(this IOrderedDictionary dict, object key, int destIndex) {
        object? value = dict[key];
        dict.Remove(key);
        dict.Insert(destIndex, key, value);
    }
}
