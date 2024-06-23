using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SpikysLib.Extensions;

namespace SpikysLib.DataStructures;

public class JoinedLists<T> : IList<T>, IReadOnlyList<T> {
    public ReadOnlyCollection<IList<T>> Lists => new(_lists);
    public T this[int index] {
        get {
            var i = InnerIndex(index);
            return Lists[i.list][i.index];
        }
        set {
            var i = InnerIndex(index);
            Lists[i.list][i.index] = value;
        }
    }
    public int Count {
        get {
            int c = 0;
            foreach (IList<T> list in Lists) c += list.Count;
            return c;
        }
    }

    public bool IsReadOnly {
        get {
            foreach (IList<T> list in Lists) {
                if (!list.IsReadOnly) return false;
            }
            return true;
        }
    }

    public JoinedLists(params IList<T>[] lists) => _lists = lists;

    public bool Contains(T item) {
        foreach (IList<T> list in Lists) if (list.Contains(item)) return true;
        return false;
    }
    public int IndexOf(T item) {
        int s = 0;
        foreach (IList<T> list in Lists) {
            int i = list.IndexOf(item);
            if (i != -1) return i;
            s += list.Count;
        }
        return -1;
    }

    public void Insert(int index, T item) => Insert(index, item, false);
    public void Insert(int index, T item, bool addIfJunction) {
        var i = InnerIndex(index);
        if (i.list > 0 && i.index == 0 && addIfJunction) Lists[i.list - 1].Add(item);
        else Lists[i.list].Insert(i.index, item);
    }
    public void Add(T item) => Lists[^1].Add(item);
    
    public bool Remove(T item) {
        foreach (IList<T> list in Lists) if (list.Remove(item)) return true;
        return false;
    }
    public void RemoveAt(int index) {
        var i = InnerIndex(index);
        Lists[i.list].RemoveAt(i.index);
    }
    public void Clear() {
        foreach (IList<T> list in Lists) list.Clear();
    }

    public (int list, int index) InnerIndex(int index) {
        int l = 0;
        while (index >= Lists[l].Count) index -= Lists[l++].Count;
        return (l, index);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        foreach (IList<T> list in Lists) {
            list.CopyTo(array, arrayIndex);
            arrayIndex += list.Count;
        }
    }

    public IEnumerator<T> GetEnumerator() {
        foreach (IList<T> list in Lists) {
            foreach (T item in list) yield return item;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly IList<T>[] _lists;
}

[Obsolete($"use {nameof(JoinedLists<object>)} instead", true)]
public readonly record struct Joined<TList, T> : IList<T>, IReadOnlyList<T> where TList: IList<T> {
    public TList[] Lists => _joinedLists.Lists.Select(l => (TList)l).ToArray();
    public T this[int index] { get => _joinedLists[index]; set => _joinedLists[index] = value; }
    public int Count => _joinedLists.Count;
    
    public bool IsReadOnly => _joinedLists.IsReadOnly;
    
    public Joined(params TList[] lists) => _joinedLists = new([..lists]);

    public bool Contains(T item) => _joinedLists.Contains(item);
    public int IndexOf(T item) => _joinedLists.IndexOf(item);

    public void Insert(int index, T item) => _joinedLists.Insert(index, item);
    public void Insert(int index, T item, bool addIfJunction) => _joinedLists.Insert(index, item, addIfJunction);
    public void Add(T item) => _joinedLists.Add(item);
    
    public bool Remove(T item) => _joinedLists.Remove(item);
    public void RemoveAt(int index) => _joinedLists.RemoveAt(index);
    public void Clear() => _joinedLists.Clear();

    public (int list, int index) InnerIndex(int index) => _joinedLists.InnerIndex(index);

    public void CopyTo(T[] array, int arrayIndex) => _joinedLists.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _joinedLists.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _joinedLists.GetEnumerator();

    private readonly JoinedLists<T> _joinedLists;

    public static implicit operator Joined<TList, T>(TList list) => new(list);
}

public sealed class ListIndices<T> : IList<T>, IReadOnlyList<T> {

    public IList<T> List { get; }
    public IReadOnlyList<int> Indices { get; }
    public bool ExcludeIndices { get; }

    public ListIndices(IList<T> list) : this(list, Array.Empty<int>(), true) { }
    public ListIndices(IList<T> list, params int[] indices) : this(list, (IReadOnlyList<int>)indices) { }
    public ListIndices(IList<T> list, bool excludeIndices, params int[] indices) : this (list, indices, excludeIndices) { }
    public ListIndices(IList<T> list, IReadOnlyList<int> indices, bool excludeIndices = false) {
        List = list;
        Indices = indices;
        ExcludeIndices = excludeIndices;
    }

    public T this[int index] { get => List[ToInnerIndex(index)]; set => List[ToInnerIndex(index)] = value; }

    public int Count => ExcludeIndices ? (List.Count - Indices.Count) : Indices.Count;

    public bool IsReadOnly => List.IsReadOnly;

    private int ToInnerIndex(int index) {
        if (!ExcludeIndices) return Indices[index];
        int i = 0;
        while (i < Indices.Count && Indices[i] <= index) i++;
        return index + i;
    }

    public int FromInnerIndex(int index) {
        if (!ExcludeIndices) return Indices.FindIndex(i => i == index);
        int i;
        for (i = 0; i < Indices.Count && Indices[i] <= index; i++) {
            if (Indices[i] == index) return -1;
        }
        return index - i;
    }

    public bool Contains(T item) => IndexOf(item) != -1;
    public int IndexOf(T item) {
        int i = 0;
        foreach (int index in GetIndices()) {
            if (Equals(item, List[index])) return i;
            i++;
        }
        return -1;
    }

    public void CopyTo(T[] array, int arrayIndex) {
        foreach (T item in this) array[arrayIndex++] = item;
    }

    public IEnumerable<int> GetIndices() {
        if (!ExcludeIndices) {
            foreach (int i in Indices) yield return i;
        } else {
            int j = 0;
            for (int i = 0; i < List.Count; i++) {
                if (j < Indices.Count && Indices[j] < i) j++;
                if (j < Indices.Count && Indices[j] == i) continue;
                yield return i;
            }
        }
    }

    public IEnumerator<T> GetEnumerator() {
        foreach (int i in GetIndices()) yield return List[i]; 
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection<T>.Clear() => throw new NotSupportedException();
    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
    void ICollection<T>.Add(T item) => throw new NotSupportedException();
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();
}