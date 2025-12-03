using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;

namespace SpikysLib.DataStructures;

public class DictionaryWithStats<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull {

    public TValue this[TKey key] {
        get => TryGetValue(key, out TValue? value) ? value : throw new KeyNotFoundException();
        set { Remove(key); Add(key, value); }
    }
    
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        Get++;
        bool res = _dict.TryGetValue(key, out value);
        if(res) Hits++;
        return res;
    }


    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
    public bool TryAdd(TKey key, TValue value) {
        if (ContainsKey(key)) return false;
        Add(key, value);
        return true;
    }
    public void Add(TKey key, TValue value) {
        if(EstimateValueSize is not null) EstimatedSize += EstimateValueSize(value);
        _dict.Add(key, value);
    }
    public bool Remove(TKey key) {
        if (!ContainsKey(key)) return false;
        if(EstimateValueSize is not null) EstimatedSize -= EstimateValueSize(_dict[key]);
        return _dict.Remove(key);
    }


    public void Clear() {
        ClearStats();
        EstimatedSize = 0;
        _dict.Clear();
    }
    public void ClearStats() {
        LastClearTime = Main.GlobalTimeWrappedHourly;
        Get = 0;
        Hits = 0;
    }


    public long EstimatedSize { get; private set; }
    public Func<TValue, long>? EstimateValueSize { get; init; }

    public float LastClearTime { get; private set; }
    public long Get { get; private set; }
    public long Hits { get; private set; }

    public ICollection<TKey> Keys => _dict.Keys;
    public ICollection<TValue> Values => _dict.Values;
    public int Count => _dict.Count;

    public string Stats() => $"{Hits} hits ({(Get * Hits == 0 ? 100 : Hits * 100f / Get):F2}%), {Count} values {(EstimateValueSize is not null ? $" (~{EstimatedSize}B)" : string.Empty)} in {Main.GlobalTimeWrappedHourly - LastClearTime:F0}s";
    
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)this).IsReadOnly;
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Add(item);
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item);
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this).Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this).GetEnumerator();

    private readonly Dictionary<TKey, TValue> _dict = new();
}