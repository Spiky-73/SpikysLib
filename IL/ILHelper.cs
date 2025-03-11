using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace SpikysLib.IL;

public static class ILHelper {

    public static bool SaferMatchCall(this Instruction inst, MethodInfo method) => SaferMatch(() => inst.MatchCall(method));
    public static bool SaferMatchCall(this Instruction inst, Type type, string name) => SaferMatch(() => inst.MatchCall(type, name));
    public static bool SaferMatchCallvirt(this Instruction inst, MethodInfo method) => SaferMatch(() => inst.MatchCallvirt(method));
    public static bool SaferMatchCallvirt(this Instruction inst, Type type, string name) => SaferMatch(() => inst.MatchCallvirt(type, name));
    private static bool SaferMatch(Func<bool> cb) {
        try { return cb(); }
        catch (InvalidCastException) { return false; }
    }

    public static ILCursor GotoNextLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => cursor.GotoNextLoc(MoveType.Before, out value, predicate, def);
    public static ILCursor GotoNextLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) => GotoLoc(cursor, cursor.TryGotoNext, moveType, out value, predicate, def);
    public static ILCursor GotoPrevLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => cursor.GotoPrevLoc(MoveType.Before, out value, predicate, def);
    public static ILCursor GotoPrevLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) => GotoLoc(cursor, cursor.TryGotoPrev, moveType, out value, predicate, def);
    private static ILCursor GotoLoc(ILCursor cursor, TryGoto finder, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (finder(moveType, i => i.MatchStloc(out loc) && predicate(i))) value = loc;
        else throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        return cursor;
    }
    private delegate bool TryGoto(MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates);

    public static void FindPrevLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) => FindLoc(cursor.TryFindPrev, out c, out value, predicate, def);
    public static void FindNextLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) => FindLoc(cursor.TryFindNext, out c, out value, predicate, def);
    private static void FindLoc(TryFind finder, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (finder(out ILCursor[] cs, i => i.MatchStloc(out loc) && predicate(i))) value = loc;
        else throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        c = cs[0];
    }
    private delegate bool TryFind(out ILCursor[] cursors, params Func<Instruction, bool>[] predicates);
}
