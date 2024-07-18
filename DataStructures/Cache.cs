using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib.DataStructures;

[Obsolete($"use {nameof(IGeneratedDictionary<TItem,TValue>)} instead")]
public interface ICache<TItem, TValue> : IDictionary<TItem, TValue> {
    TValue GetOrAdd(TItem key);
}

[Obsolete($"use {nameof(GeneratedDictionary<TKey, TValue>)} instead", true)]
public sealed class Cache<TKey, TValue> : DictionaryWithStats<TKey, TValue>, ICache<TKey, TValue> where TKey : notnull{

    public Cache(Func<TKey, TValue> builder): base() {
        Builder = builder;
    }

    public TValue GetOrAdd(TKey key) {
        if (TryGetValue(key, out TValue? value)) return value;
        return this[key] = Builder(key);
    }

    public Func<TKey, TValue> Builder { get; }
}

[Obsolete($"use {nameof(GeneratedDictionary<TItem, TKey, TValue>)} instead", true)]
public sealed class Cache<TItem, TKey, TValue> : DictionaryWithStats<TKey, TValue>, ICache<TItem, TValue> where TKey : notnull {

    public Cache(Func<TItem, TKey> indexer, Func<TItem, TValue> builder) {
        Indexer = indexer;
        Builder = builder;
    }

    public TValue GetOrAdd(TItem item) {
        TKey key = Indexer(item);
        if (TryGetValue(key, out TValue? value)) return value;
        return this[key] = Builder(item);
    }

    public TValue this[TItem item] { get => base[Indexer(item)]; set => base[Indexer(item)] = value; }
    public bool TryGetValue(TItem item, [MaybeNullWhen(false)] out TValue value) => TryGetValue(Indexer(item), out value);
    public void Add(TItem item, TValue value) => Add(Indexer(item), value);
    public bool Remove(TItem item) => Remove(Indexer(item));
    public bool ContainsKey(TItem item) => ContainsKey(Indexer(item));

    public Func<TItem, TKey> Indexer { get; }
    public Func<TItem, TValue> Builder { get; }

    ICollection<TValue> IDictionary<TItem, TValue>.Values => Values;
    void ICollection<KeyValuePair<TItem, TValue>>.Add(KeyValuePair<TItem, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Add(new(Indexer(item.Key), item.Value));
    bool ICollection<KeyValuePair<TItem, TValue>>.Contains(KeyValuePair<TItem, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(new(Indexer(item.Key), item.Value));
    bool ICollection<KeyValuePair<TItem, TValue>>.Remove(KeyValuePair<TItem, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Remove(new KeyValuePair<TKey, TValue>(Indexer(item.Key), item.Value));
    bool ICollection<KeyValuePair<TItem, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)this).IsReadOnly;

    ICollection<TItem> IDictionary<TItem, TValue>.Keys => throw new NotSupportedException();
    IEnumerator<KeyValuePair<TItem, TValue>> IEnumerable<KeyValuePair<TItem, TValue>>.GetEnumerator() => throw new NotSupportedException();
    void ICollection<KeyValuePair<TItem, TValue>>.CopyTo(KeyValuePair<TItem, TValue>[] array, int arrayIndex) => throw new NotSupportedException();
}