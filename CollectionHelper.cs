using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib;

public static class CollectionHelper {

    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate) {
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
}
