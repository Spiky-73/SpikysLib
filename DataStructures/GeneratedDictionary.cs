using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib.DataStructures;

public interface IGeneratedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
    TValue GetValue(TKey key);
    bool Remove(TKey key);
    void Clear();
    Func<TKey, TValue> Factory { get; }
}

public sealed class GeneratedDictionary<TKey, TValue> : IGeneratedDictionary<TKey, TValue> where TKey : notnull{

    public GeneratedDictionary(Func<TKey, TValue> generator) : this(new Dictionary<TKey, TValue>(), generator) { }
    public GeneratedDictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> factory): base() {
        _dictionary = dictionary;
        Factory = factory;
    }

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);
    
    public TValue this[TKey key] => GetValue(key);
    public TValue GetValue(TKey key) {
        if (_dictionary.TryGetValue(key, out TValue? value)) return value;
        _dictionary.Add(key, Factory(key));
        return _dictionary[key];
    }

    public bool Remove(TKey key) => _dictionary.Remove(key);
    public void Clear() => _dictionary.Clear();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

    public Func<TKey, TValue> Factory { get; }

    public IEnumerable<TKey> Keys => _dictionary.Keys;
    public IEnumerable<TValue> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    private readonly IDictionary<TKey, TValue> _dictionary;
}

public interface IGeneratedDictionary<TItem, TKey, TValue> : IGeneratedDictionary<TItem, TValue>, IReadOnlyDictionary<TItem, TValue> {
    Func<TItem, TKey> Indexer { get; }
}

public sealed class GeneratedDictionary<TItem, TKey, TValue> : IGeneratedDictionary<TItem, TKey, TValue> where TKey : notnull {

    public GeneratedDictionary(Func<TItem, TKey> indexer, Func<TItem, TValue> generator) : this(new Dictionary<TKey, TValue>(), indexer, generator) { }
    public GeneratedDictionary(IDictionary<TKey, TValue> dictionary, Func<TItem, TKey> indexer, Func<TItem, TValue> generator) {
        Indexer = indexer;
        Factory = generator;
        _dictionary = dictionary;
    }

    public Func<TItem, TKey> Indexer { get; }
    public Func<TItem, TValue> Factory { get; }

    public bool ContainsKey(TItem item) => ContainsKey(Indexer(item));
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGetValue(TItem item, [MaybeNullWhen(false)] out TValue value) => TryGetValue(Indexer(item), out value);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);
    
    public TValue this[TItem item] => GetValue(item);
    public TValue this[TKey key] => GetValue(key);
    public TValue GetValue(TItem item) {
        TKey key = Indexer(item);
        if (_dictionary.TryGetValue(key, out TValue? value)) return value;
        _dictionary.Add(key, Factory(item));
        return _dictionary[key];
    }
    public TValue GetValue(TKey key) => _dictionary[key];

    public bool Remove(TItem item) => Remove(Indexer(item));
    public bool Remove(TKey key) => _dictionary.Remove(key);
    public void Clear() => _dictionary.Clear();

    public IEnumerable<TKey> Keys => _dictionary.Keys;
    public IEnumerable<TValue> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

    private IDictionary<TKey, TValue> _dictionary;

    IEnumerator<KeyValuePair<TItem, TValue>> IEnumerable<KeyValuePair<TItem, TValue>>.GetEnumerator() => throw new NotSupportedException();
    IEnumerable<TItem> IReadOnlyDictionary<TItem, TValue>.Keys => throw new NotSupportedException();
}