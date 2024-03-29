using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SpikysLib.Extensions;

namespace SpikysLib.DataStructures;

public readonly record struct Range(int Start, int End) : IReadOnlyList<int>, IReadOnlySet<int> {
    public Range(int value) : this(value, value) {}
    public static Range FromCount(int start, int count) => new(start, start + count - 1);
    public readonly int Count => End - Start + 1;

    public int this[int index] => index < Count ? Start + index : throw new IndexOutOfRangeException();

    public bool Contains(int item) => Start <= item && item <= End;

    public IEnumerator<int> GetEnumerator() {
        for (int i = 0; i < Count; i++) yield return this[i];
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool IsSupersetOf(IEnumerable<int> other) {
        if (other is Range range) return Start <= range.Start && range.End <= End;
        Range self = this;
        return !other.Exist(i => !self.Contains(i), out int count) && Count >= count;
    }
    public bool IsProperSupersetOf(IEnumerable<int> other) {
        if (other is Range range) return Count > range.Count && IsSupersetOf(range);
        Range self = this;
        return !other.Exist(i => !self.Contains(i), out int count) && Count > count;
    }
    public bool IsSubsetOf(IEnumerable<int> other) {
        if (other is Range range) range.IsSubsetOf(this);
        int total = 0;
        foreach (int item in other) {
            if (Contains(item)) total++;
        }
        return total == Count;
    }
    public bool IsProperSubsetOf(IEnumerable<int> other) {
        if (other is Range range) range.IsProperSubsetOf(this);
        int total = 0;
        int count = 0;
        foreach (int item in other) {
            if (Contains(item)) total++;
            count++;
        }
        return total == Count && count > Count;
    }
    public bool Overlaps(IEnumerable<int> other) {
        if (other is Range range) return (range.Start <= End && Start <= range.End) || (Start <= range.End && range.Start <= End);
        foreach (int item in other){
            if (Contains(item)) return true;
        }
        return false;
    }
    public bool SetEquals(IEnumerable<int> other){
        if (other is Range range) return range.Start == Start && range.End == End;
        int total = 0;
        foreach (int item in other){
            if (!Contains(item)) return false;
            total++;
        }
        return total == Count;
    }
}

public sealed class RangeSet : ISet<int>, IReadOnlySet<int> {

    public RangeSet() {
        Count = 0;
        _ranges = new();
    }

    public RangeSet(IEnumerable<int> values): this() => AddRange(values);

    public ReadOnlyCollection<Range> Ranges => new(_ranges);

    public int Count { get; private set; }

    public void AddRange(IEnumerable<int> enumerable) { foreach (int i in enumerable) Add(i); }
    public bool Add(int item) => Add(new Range(item, item));
    public bool Add(Range range) {
        int i = FindInsertIndex(range.Start);
        if (i != 0 && _ranges[i - 1].Start <= range.Start && range.End <= _ranges[i - 1].End) return false;
        if (i == 0 || range.Start - _ranges[i - 1].End > 1) _ranges.Insert(i, range);
        else if (range.End > _ranges[--i].End) {
            Count -= _ranges[i].Count;
            _ranges[i] = new(_ranges[i].Start, range.End);
        }
        
        int j = i + 1;
        while (j < _ranges.Count && _ranges[j].End <= _ranges[i].End) {
            Count -= _ranges[j].Count;
            j++;
        }
        _ranges.RemoveRange(i+1, j - i - 1);

        if (i != _ranges.Count-1 && _ranges[i+1].Start - _ranges[i].End <= 1) {
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
            if (item != _ranges[i - 1].End) _ranges.Insert(i, new(item + 1, _ranges[i - 1].End));
            _ranges[i - 1] = new(_ranges[i - 1].Start, item - 1);
        }
        Count--;
        return true;
    }

    public bool Contains(int item) => Contains(new Range(item), out _);
    public bool Contains(Range range) => Contains(range, out _);
    private bool Contains(Range range, out int index) {
        index = FindInsertIndex(range.Start);
        return index != 0 && range.End <= _ranges[index-1].End;
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
        foreach (int item in other) Add(item);
    }

    public void IntersectWith(IEnumerable<int> other) {
        RangeSet toKeep = new();
        foreach (int item in other) {
            if (Contains(item)) toKeep.Add(item);
        };
        Clear();
        foreach(Range r in toKeep.Ranges) Add(r);
    }

    public void SymmetricExceptWith(IEnumerable<int> other) {
        List<int> toRemove = new();
        foreach (int item in other) {
            if (Contains(item)) toRemove.Add(item);
            else Add(item);
        };
        foreach (int r in toRemove) Remove(r);
    }

    public bool IsSupersetOf(IEnumerable<int> other) {
        if (other is RangeSet set) {
            foreach (Range range in set.Ranges) {
                if (!Contains(range)) return false;
            }
            return true;
        }
        return !other.Exist(i => !Contains(i), out int count) && Count >= count;
    }
    public bool IsProperSupersetOf(IEnumerable<int> other) {
        if (other is RangeSet set) return Count > set.Count && IsSupersetOf(set);
        return !other.Exist(i => !Contains(i), out int count) && Count > count;
    }
    public bool IsSubsetOf(IEnumerable<int> other) {
        if (other is RangeSet set) set.IsSubsetOf(this);
        int total = 0;
        foreach (int item in other) {
            if (Contains(item)) total++;
        }
        return total == Count;
    }
    public bool IsProperSubsetOf(IEnumerable<int> other) {
        if (other is RangeSet set) set.IsProperSubsetOf(this);
        int total = 0;
        int count = 0;
        foreach (int item in other) {
            if (Contains(item)) total++;
            count++;
        }
        return total == Count && count > Count;
    }
    public bool Overlaps(IEnumerable<int> other) {
        if (other is RangeSet set) {
            foreach (Range r1 in set._ranges) {
                foreach (Range r2 in set._ranges) {
                    if (r1.Overlaps(r2)) return true;
                }
            }
            return false;
        }
        foreach (int item in other) {
            if (Contains(item)) return true;
        }
        return false;
    }
    public bool SetEquals(IEnumerable<int> other) {
        if (other is RangeSet set) {
            if (_ranges.Count != set.Ranges.Count || Count != set.Count) return false;
            for (int i = 0; i < _ranges.Count; i++) {
                if (!_ranges[i].SetEquals(set._ranges[i])) return false;
            }
            return true;
        }
        int total = 0;
        foreach (int item in other) {
            if (!Contains(item)) return false;
            total++;
        }
        return total == Count;
    }

    void ICollection<int>.Add(int item) => Add(item);
    public void CopyTo(int[] array, int arrayIndex) {
        foreach (int item in this) array[arrayIndex++] = item;
    }


    private readonly List<Range> _ranges;
}