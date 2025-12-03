using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SpikysLib.Collections;
using System.Linq;

namespace SpikysLib.DataStructures;

public readonly record struct Range(int Start, int End) : IReadOnlyList<int>, IReadOnlySet<int> {
    public Range(int value) : this(value, value+1) {}
    public static Range FromCount(int start, int count) => new(start, start + count);
    
    public readonly int Count => End - Start;

    public int this[int index] => index < Count ? Start + index : throw new IndexOutOfRangeException();

    public bool Contains(int item) => Start <= item && item < End;

    public bool IsSubsetOf(IEnumerable<int> other) {
        if (other is Range range) return range.Start <= Start && End <= range.End;
        return !this.Exist(i => !other.Contains(i));
    }
    public bool IsSupersetOf(IEnumerable<int> other) {
        if (other is Range range) return range.IsSubsetOf(this);
        Range self = this;
        return !other.Exist(i => !self.Contains(i));
    }

    public bool IsProperSubsetOf(IEnumerable<int> other) {
        if (other is Range range) return Count < range.Count && IsSubsetOf(range);
        return !this.Exist(i => !other.Contains(i), out int count) && Count < count;
    }
    public bool IsProperSupersetOf(IEnumerable<int> other) {
        if (other is Range range) return range.IsProperSubsetOf(this);
        Range self = this;
        return !other.Exist(i => !self.Contains(i), out int count) && Count > count;
    }

    public bool Overlaps(IEnumerable<int> other) {
        if (other is Range range) return Start < range.End && range.Start < End;
        return other.Exist(Contains);
    }

    public bool SetEquals(IEnumerable<int> other){
        if (other is Range range) return range.Start == Start && range.End == End;
        Range self = this;
        return !other.Exist(i => !self.Contains(i), out int count) && Count == count;
    }

    public IEnumerator<int> GetEnumerator() {
        for (int i = 0; i < Count; i++) yield return Start + i;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class RangeSet : ISet<int>, IReadOnlySet<int>{

    public RangeSet() {
        Count = 0;
        _ranges = new();
    }

    public RangeSet(IEnumerable<int> values): this() => AddRange(values);

    public ReadOnlyCollection<Range> Ranges => new(_ranges);

    public int Count { get; private set; }

    public void AddRange(IEnumerable<int> enumerable) { foreach (int i in enumerable) Add(i); }
    public bool Add(int item) => Add(new Range(item));
    public bool Add(Range range) {
        if (Contains(range, out int i)) return false;
        if (i == 0 || _ranges[i - 1].End < range.Start) _ranges.Insert(i, range);
        else {
            Count -= _ranges[--i].Count;
            _ranges[i] = new(_ranges[i].Start, range.End);
        }
        
        int j = i + 1;
        while (j < _ranges.Count && _ranges[j].End <= _ranges[i].End) {
            Count -= _ranges[j].Count;
            j++;
        }
        _ranges.RemoveRange(i+1, j - i - 1);

        if (i != _ranges.Count-1 && _ranges[i+1].Start < _ranges[i].End) {
            _ranges[i] = new(_ranges[i].Start, _ranges[i+1].End);
            Count -= _ranges[i + 1].Count;
            _ranges.RemoveAt(i+1);
        }
        Count += _ranges[i].Count;
        return true;
    }

    public bool Remove(int item) {
        if (!Contains(new Range(item), out int i)) return false;
        if (_ranges[i - 1].Count == 1) _ranges.RemoveAt(i - 1);
        else if (item == _ranges[i - 1].Start) _ranges[i - 1] = new(item + 1, _ranges[i - 1].End);
        else {
            if (item + 1 != _ranges[i - 1].End) _ranges.Insert(i, new(item + 1, _ranges[i - 1].End));
            _ranges[i - 1] = new(_ranges[i - 1].Start, item);
        }
        Count--;
        return true;
    }

    public bool Contains(int item) => Contains(new Range(item), out _);
    public bool Contains(Range range) => Contains(range, out _);
    private bool Contains(Range range, out int index) {
        index = FindInsertIndex(range.Start);
        return index != 0 && _ranges[index - 1].IsSupersetOf(range);
    }
    private int FindInsertIndex(int item) {
        for (int i = 0; i < _ranges.Count; i++) if (item < _ranges[i].Start) return i;
        return _ranges.Count;
    }

    public void Clear() {
        _ranges.Clear();
        Count = 0;
    }

    public IEnumerator<int> GetEnumerator() {
        foreach (Range range in _ranges){
            foreach (int value in range) yield return value;
        }
    }

    bool ICollection<int>.IsReadOnly => false;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ExceptWith(IEnumerable<int> other) {
        foreach (int item in other) Remove(item);
    }
    public void UnionWith(IEnumerable<int> other) {
        if (other is RangeSet set) foreach (Range range in set.Ranges) Add(range);
        else foreach (int item in other) Add(item);
    }

    public void IntersectWith(IEnumerable<int> other) => ExceptWith(this.Where(i => !other.Contains(i)));
    public void SymmetricExceptWith(IEnumerable<int> other) {
        UnionWith(other);
        ExceptWith(other.Where(Contains));
    }

    public bool IsSubsetOf(IEnumerable<int> other) {
        if (other is RangeSet set) return !_ranges.Exist(r => !set.Contains(r));
        return !this.Exist(i => !other.Contains(i));
    }
    public bool IsSupersetOf(IEnumerable<int> other) {
        if (other is RangeSet set) return !set.IsSubsetOf(this);
        return !other.Exist(i => !Contains(i));
    }
    public bool IsProperSubsetOf(IEnumerable<int> other) {
        if (other is RangeSet set) return Count < set.Count && IsSubsetOf(set);
        return !this.Exist(i => !other.Contains(i), out int count) && Count < count;
    }
    public bool IsProperSupersetOf(IEnumerable<int> other) {
        if (other is RangeSet set) return set.IsProperSubsetOf(this);
        return !other.Exist(i => !Contains(i), out int count) && Count > count;
    }
    public bool Overlaps(IEnumerable<int> other) {
        if (other is RangeSet set) {
            foreach (Range r1 in _ranges) {
                if (set._ranges.Exist(r2 => r1.Overlaps(r2))) return true;
            }
            return false;
        }
        return other.Exist(Contains);
    }
    public bool SetEquals(IEnumerable<int> other) {
        if (other is RangeSet set) return _ranges.Count == set._ranges.Count && Count == set.Count && !_ranges.Zip(set._ranges).Exist((a) => a.First != a.Second);
        RangeSet self = this;
        return !other.Exist(i => !self.Contains(i), out int count) && Count == count;
    }

    void ICollection<int>.Add(int item) => Add(item);
    public void CopyTo(int[] array, int arrayIndex) {
        foreach (int item in this) array[arrayIndex++] = item;
    }

    private readonly List<Range> _ranges;
}