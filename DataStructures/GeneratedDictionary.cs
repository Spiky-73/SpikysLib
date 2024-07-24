using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpikysLib.DataStructures;

public interface IGeneratedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
    TValue GetValue(TKey key);
    bool Remove(TKey key);
    void Clear();
    Func<TKey, TValue> Generator { get; }
}

public sealed class GeneratedDictionary<TKey, TValue> : IGeneratedDictionary<TKey, TValue> where TKey : notnull{

    public GeneratedDictionary(Func<TKey, TValue> generator) : this(new Dictionary<TKey, TValue>(), generator) { }
    public GeneratedDictionary(IDictionary<TKey, TValue> dictionnary, Func<TKey, TValue> generator): base() {
        _dictionnary = dictionnary;
        Generator = generator;
    }

    public bool ContainsKey(TKey key) => _dictionnary.ContainsKey(key);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionnary.TryGetValue(key, out value);
    
    public TValue this[TKey key] => GetValue(key);
    public TValue GetValue(TKey key) {
        if (_dictionnary.TryGetValue(key, out TValue? value)) return value;
        _dictionnary.Add(key, Generator(key));
        return _dictionnary[key];
    }

    public bool Remove(TKey key) => _dictionnary.Remove(key);
    public void Clear() => _dictionnary.Clear();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionnary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionnary).GetEnumerator();

    public Func<TKey, TValue> Generator { get; }

    public IEnumerable<TKey> Keys => _dictionnary.Keys;
    public IEnumerable<TValue> Values => _dictionnary.Values;

    public int Count => _dictionnary.Count;

    private readonly IDictionary<TKey, TValue> _dictionnary;
}

public interface IGeneratedDictionary<TItem, TKey, TValue> : IGeneratedDictionary<TItem, TValue>, IReadOnlyDictionary<TItem, TValue> {
    Func<TItem, TKey> Indexer { get; }
}

public sealed class GeneratedDictionary<TItem, TKey, TValue> : IGeneratedDictionary<TItem, TKey, TValue> where TKey : notnull {

    public GeneratedDictionary(Func<TItem, TKey> indexer, Func<TItem, TValue> generator) : this(new Dictionary<TKey, TValue>(), indexer, generator) { }
    public GeneratedDictionary(IDictionary<TKey, TValue> dictionnary, Func<TItem, TKey> indexer, Func<TItem, TValue> generator) {
        Indexer = indexer;
        Generator = generator;
        _dictionnary = dictionnary;
    }

    public Func<TItem, TKey> Indexer { get; }
    public Func<TItem, TValue> Generator { get; }

    public bool ContainsKey(TItem item) => ContainsKey(Indexer(item));
    public bool ContainsKey(TKey key) => _dictionnary.ContainsKey(key);
    public bool TryGetValue(TItem item, [MaybeNullWhen(false)] out TValue value) => TryGetValue(Indexer(item), out value);
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionnary.TryGetValue(key, out value);
    
    public TValue this[TItem item] => GetValue(item);
    public TValue this[TKey key] => GetValue(key);
    public TValue GetValue(TItem item) {
        TKey key = Indexer(item);
        if (_dictionnary.TryGetValue(key, out TValue? value)) return value;
        _dictionnary.Add(key, Generator(item));
        return _dictionnary[key];
    }
    public TValue GetValue(TKey key) => _dictionnary[key];

    public bool Remove(TItem item) => Remove(Indexer(item));
    public bool Remove(TKey key) => _dictionnary.Remove(key);
    public void Clear() => _dictionnary.Clear();

    public IEnumerable<TKey> Keys => _dictionnary.Keys;
    public IEnumerable<TValue> Values => _dictionnary.Values;

    public int Count => _dictionnary.Count;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionnary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionnary).GetEnumerator();

    private IDictionary<TKey, TValue> _dictionnary;

    IEnumerator<KeyValuePair<TItem, TValue>> IEnumerable<KeyValuePair<TItem, TValue>>.GetEnumerator() => throw new NotSupportedException();
    IEnumerable<TItem> IReadOnlyDictionary<TItem, TValue>.Keys => throw new NotSupportedException();
}