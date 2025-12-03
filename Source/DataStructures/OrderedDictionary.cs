using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SpikysLib.Configs.UI;
using Terraria.ModLoader.Config;

namespace SpikysLib.DataStructures;

public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue> {
    new TValue this[int index] { get; set; }
    void Insert(int index, TKey key, TValue value);

    object? IOrderedDictionary.this[int index] {
        get => this[index];
        set => this[index] = (TValue)value!;
    }
    object? IDictionary.this[object key] {
        get => this[(TKey)key];
        set => this[(TKey)key] = (TValue)value!;
    }
    ICollection IDictionary.Keys => ((IDictionary<TKey, TValue>)this).Keys.ToArray();
    ICollection IDictionary.Values => ((IDictionary<TKey, TValue>)this).Values.ToArray();
    void IOrderedDictionary.Insert(int index, object key, object? value) => Insert(index, (TKey)key!, (TValue)value!);
    void IDictionary.Add(object key, object? value) => Add((TKey)key, (TValue)value!);
    bool IDictionary.Contains(object key) => ContainsKey((TKey)key);
    void IDictionary.Remove(object key) => Remove((TKey)key);
}

[CustomModConfigItem(typeof(DictionaryElement))]
public sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue> where TKey : notnull {

    public OrderedDictionary() : this(null, null) {}
    public OrderedDictionary(IEqualityComparer<TKey>? comparer) : this(null, comparer) { }
    public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>>? collection, IEqualityComparer<TKey>? comparer) {
        _keyedCollection = new(comparer);
        if (collection is null) return;
        foreach (var kvp in collection) _keyedCollection.Add(kvp);
    }

    public bool ContainsKey(TKey key) => _keyedCollection.Contains(key);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        value = default;
        if (!_keyedCollection.TryGetValue(key, out var kvp)) return false;
        value = kvp.Value;
        return true;
    }

    public void Add(TKey key, TValue value) => _keyedCollection.Add(new(key, value));
    public void Insert(int index, TKey key, TValue value) => _keyedCollection.Insert(index, new(key, value));

    public bool Remove(TKey key) => _keyedCollection.Remove(key);
    public void RemoveAt(int index) => _keyedCollection.RemoveAt(index);
    public void Clear() => _keyedCollection.Clear();

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _keyedCollection.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _keyedCollection.GetEnumerator();

    public TValue this[int index] { get => _keyedCollection[index].Value; set => _keyedCollection[index] = new(_keyedCollection[index].Key, value); }
    public TValue this[TKey key] {
        get => _keyedCollection[key].Value;
        set {
            if (!_keyedCollection.TryGetValue(key, out var kvp)) Add(key, value);
            else _keyedCollection[_keyedCollection.IndexOf(kvp)] = new(key, value);
        }
    }
    public int Count => _keyedCollection.Count;
    public ReadOnlyCollection<TKey> Keys => new(_keyedCollection.Select(i => i.Key).ToArray());
    public ReadOnlyCollection<TValue> Values => new(_keyedCollection.Select(i => i.Value).ToArray());

    private readonly KeyValuePairCollection<TKey, TValue> _keyedCollection;

    bool IDictionary.IsFixedSize => ((ICollection<KeyValuePair<TKey, TValue>>)_keyedCollection).IsReadOnly;
    bool IDictionary.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_keyedCollection).IsReadOnly;
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_keyedCollection).IsReadOnly;
    bool ICollection.IsSynchronized => ((ICollection)_keyedCollection).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)_keyedCollection).SyncRoot;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => _keyedCollection.Contains(item);
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => _keyedCollection.Add(item);
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => _keyedCollection.Remove(item);

    void ICollection.CopyTo(Array array, int index) => ((ICollection)_keyedCollection).CopyTo(array, index);

    IDictionaryEnumerator IOrderedDictionary.GetEnumerator() => ((IDictionary)this).GetEnumerator();
    IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator<TKey, TValue>(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}