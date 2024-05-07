using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SpikysLib;

public static class MathX {

    public static T Min<T>(params T[] values) where T : IComparable<T> => values.Min()!;
    public static T Max<T>(params T[] values) where T : IComparable<T> => values.Max()!;

    [Flags] public enum InclusionFlag { Min = 0x01, Max = 0x10, Both = Min | Max }
    public static bool InRange<T>(T self, T min, T max, InclusionFlag flags = InclusionFlag.Both) where T : IComparable<T> => InRange(Comparer<T>.Default, self, min, max, flags);
    public static bool InRange<T>(IComparer<T> comparer, T value, T min, T max, InclusionFlag flags = InclusionFlag.Both) {
        int l = comparer.Compare(value, min);
        int r = comparer.Compare(value, max);
        return (l > 0 || (flags.HasFlag(InclusionFlag.Min) && l == 0)) && (r < 0 || (flags.HasFlag(InclusionFlag.Max) && r == 0));
    }

    public enum SnapMode { Round, Ceiling, Floor }
    public static T Snap<T>(T n, T step, SnapMode mode = SnapMode.Round) where T : INumber<T> {
        T rem = n % step;
        if (T.IsZero(rem)) return n;
        switch (mode) {
        case SnapMode.Floor:
            return n - rem;
        case SnapMode.Ceiling:
            return n + step-rem;
        default:
            T extra = step - rem;
            return n + (rem <= extra ? -rem : extra);
        }
    }

    [Obsolete("use MathX.GCD<T>(T, T) instead", true)]
    public static int GCD(int x, int y) => GCD<int>(y % x, x);
    public static T GCD<T>(T x, T y) where T : INumber<T> => T.IsZero(x) ? y : GCD(y % x, x);
}