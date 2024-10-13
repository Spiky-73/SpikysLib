using System;
using System.Collections;
using System.Collections.Generic;

namespace SpikysLib.DataStructures;

public sealed class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IDisposable {
    public DictionaryEnumerator(IDictionary<TKey, TValue> value) => _enumerator = value.GetEnumerator();

    public void Dispose() => _enumerator.Dispose();
    public void Reset() => _enumerator.Reset();
    public bool MoveNext() => _enumerator.MoveNext();
    
    public DictionaryEntry Entry => new(_enumerator.Current.Key!, _enumerator.Current.Value);
    public object Current => Entry;
    public object Key => Entry.Key;
    public object? Value => Entry.Value;

    private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;
}
