using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpikysLib.IL;

namespace SpikysLib.Extensions;

[Obsolete($"use {nameof(ILHelper)} instead", true)]
public static class IlExtensions {

    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.SaferMatchCall)} instead", true)]
    public static bool SaferMatchCall(this Instruction inst, MethodInfo method) => ILHelper.SaferMatchCall(inst, method);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.SaferMatchCall)} instead", true)]
    public static bool SaferMatchCall(this Instruction inst, Type type, string name) => ILHelper.SaferMatchCall(inst, type, name);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.SaferMatchCallvirt)} instead", true)]
    public static bool SaferMatchCallvirt(this Instruction inst, MethodInfo method) => ILHelper.SaferMatchCallvirt(inst, method);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.SaferMatchCallvirt)} instead", true)]
    public static bool SaferMatchCallvirt(this Instruction inst, Type type, string name) => ILHelper.SaferMatchCallvirt(inst, type, name);

    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.GotoNextLoc)} instead", true)]
    public static ILCursor GotoNextLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.GotoNextLoc(cursor, out value, predicate, def);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.GotoNextLoc)} instead", true)]
    public static ILCursor GotoNextLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.GotoNextLoc(cursor, moveType, out value, predicate, def);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.GotoPrevLoc)} instead", true)]
    public static ILCursor GotoPrevLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.GotoPrevLoc(cursor, out value, predicate, def);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.GotoPrevLoc)} instead", true)]
    public static ILCursor GotoPrevLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.GotoPrevLoc(cursor, moveType, out value, predicate, def);

    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.FindPrevLoc)} instead", true)]
    public static void FindPrevLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.FindPrevLoc(cursor, out c, out value, predicate, def);
    [Obsolete($"use {nameof(ILHelper)}.{nameof(ILHelper.FindNextLoc)} instead", true)]
    public static void FindNextLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) => ILHelper.FindNextLoc(cursor, out c, out value, predicate, def);
}
