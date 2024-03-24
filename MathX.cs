using System;
using System.Collections.Generic;
using System.Linq;

namespace SpikysLib;

public static class MathX {

    public static T Min<T>(params T[] values) where T : IComparable<T> => values.Min()!;
    public static T Max<T>(params T[] values) where T : IComparable<T> => values.Max()!;

    public enum InclusionFlag {
        Min = 0x01,
        Max = 0x10,
        Both = Min | Max
    }

    public static bool InRange<T>(this T self, T min, T max, InclusionFlag flags = InclusionFlag.Both) where T : IComparable<T> => InRange(Comparer<T>.Default, self, min, max, flags);
    public static bool InRange<T>(this IComparer<T> comparer, T value, T min, T max, InclusionFlag flags = InclusionFlag.Both) {
        int l = comparer.Compare(value, min);
        int r = comparer.Compare(value, max);
        return (l > 0 || (flags.HasFlag(InclusionFlag.Min) && l == 0)) && (r < 0 || (flags.HasFlag(InclusionFlag.Max) && r == 0));
    }

}