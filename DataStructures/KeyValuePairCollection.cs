using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpikysLib.DataStructures;

public sealed class KeyValuePairCollection<TKey, TValue> : KeyedCollection<TKey, KeyValuePair<TKey, TValue>> where TKey : notnull {
    public KeyValuePairCollection() : base() { }
    public KeyValuePairCollection(IEqualityComparer<TKey>? comparer) : base(comparer) { }
    public KeyValuePairCollection(IEqualityComparer<TKey>? comparer, int dictionaryCreationThreshold) : base(comparer, dictionaryCreationThreshold) { }

    protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item) => item.Key;
}