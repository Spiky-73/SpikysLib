using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SpikysLib;

[Obsolete($"use {nameof(MathHelper)} instead", true)]
public static class MathX {

    public static T Min<T>(params T[] values) where T : IComparable<T> => values.Min()!;
    public static T Max<T>(params T[] values) where T : IComparable<T> => values.Max()!;

    [Flags] public enum InclusionFlag { Min = 0x01, Max = 0x10, Both = Min | Max }
    public static bool InRange<T>(T self, T min, T max, InclusionFlag flags = InclusionFlag.Both) where T : IComparable<T> => MathHelper.InRange(self, min, max, (MathHelper.InclusionFlag)flags);
    public static bool InRange<T>(IComparer<T> comparer, T value, T min, T max, InclusionFlag flags = InclusionFlag.Both) => MathHelper.InRange(value, min, max, comparer, (MathHelper.InclusionFlag)flags);

    public enum SnapMode { Round, Ceiling, Floor }
    public static T Snap<T>(T n, T step, SnapMode mode = SnapMode.Round) where T : INumber<T> => MathHelper.Snap(n, step, (MathHelper.SnapMode)mode);

    public static T GCD<T>(T x, T y) where T : INumber<T> => MathHelper.GCD(x, y);
}