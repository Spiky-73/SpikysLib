using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using SpikysLib.Collections;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(CollectionsHelper)} instead", true)]
public static class CollectionExtensions {
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.ItemAt)} instead", true)]
    public static object ItemAt(this IEnumerable enumerable, int index) => CollectionsHelper.ItemAt(enumerable, index);

    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Exist)} instead", true)]
    public static bool Exist<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)  => CollectionsHelper.Exist(enumerable, predicate);
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Exist)} instead", true)]
    public static bool Exist<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(true)] out int count) => CollectionsHelper.Exist(enumerable, predicate, out count);

    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.FindIndex)} instead", true)]
    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate) => CollectionsHelper.FindIndex(list, predicate);
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.FindIndex)} instead", true)]
    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate) => CollectionsHelper.FindIndex(list, predicate);

    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.GetOrAdd)} instead", true)]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> builder) => CollectionsHelper.GetOrAdd(dict, key, _ => builder());
    
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Items)} instead", true)]
    public static IEnumerable<(object key, object? value)> Items(this IDictionary dict) => CollectionsHelper.Items(dict);
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Items)} instead", true)]
    public static IEnumerable<(Tkey key, TValue value)> Items<Tkey, TValue>(this IDictionary dict) where Tkey : notnull => CollectionsHelper.Items<Tkey, TValue>(dict);

    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.TryAdd)} instead", true)]
    public static bool TryAdd(this IOrderedDictionary dict, object key, object value) => CollectionsHelper.TryAdd(dict, key, value);
    
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Move)} instead", true)]
    public static void Move(this IOrderedDictionary dict, int origIndex, int destIndex) => CollectionsHelper.Move(dict, origIndex, destIndex);
    [Obsolete($"use {nameof(CollectionsHelper)}.{nameof(CollectionsHelper.Move)} instead", true)]
    public static void Move(this IOrderedDictionary dict, object key, int destIndex) => CollectionsHelper.Move(dict, key, destIndex);
}
