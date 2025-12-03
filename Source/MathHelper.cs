using System;
using System.Collections.Generic;
using System.Numerics;

namespace SpikysLib;

public static class MathHelper {
    [Flags] public enum InclusionFlag { Min = 0x01, Max = 0x10, Both = Min | Max }
    public static bool InRange<T>(T value, T min, T max, InclusionFlag flags = InclusionFlag.Both) where T : IComparable<T> => InRange(value, min, max, Comparer<T>.Default, flags);
    public static bool InRange<T>(T value, T min, T max, IComparer<T> comparer, InclusionFlag flags = InclusionFlag.Both) {
        int l = comparer.Compare(value, min);
        int r = comparer.Compare(value, max);
        return (l > 0 || (flags.HasFlag(InclusionFlag.Min) && l == 0)) && (r < 0 || (flags.HasFlag(InclusionFlag.Max) && r == 0));
    }

    public enum SnapMode { Round, Ceiling, Floor }
    public static T Snap<T>(T value, T step, SnapMode mode = SnapMode.Round) where T : INumber<T> {
        T rem = value % step;
        if (T.IsZero(rem)) return value;
        switch (mode) {
        case SnapMode.Floor:
            return value - rem;
        case SnapMode.Ceiling:
            return value + step-rem;
        default:
            T extra = step - rem;
            return value + (rem <= extra ? -rem : extra);
        }
    }

    public static T GCD<T>(T x, T y) where T : INumber<T> => T.IsZero(x) ? y : GCD(y % x, x);

    public static T Mod<T>(T x, T y) where T : INumber<T> => T.Zero < x ? (x % y) : (x % y + y);
}