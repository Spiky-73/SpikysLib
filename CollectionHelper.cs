using System;
using System.Collections.Generic;

namespace SpikysLib;

public static class CollectionHelper {

    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate) {
        for (int i = 0; i < list.Count; i++) if (predicate(list[i])) return i;
        return -1;
    }
}
