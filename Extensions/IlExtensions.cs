using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace SpikysLib.Extensions;

public static class IlExtensions {

    public static bool SaferMatchCall(this Instruction inst, MethodInfo method) => SaferMatch(() => inst.MatchCall(method));
    public static bool SaferMatchCall(this Instruction inst, Type type, string name) => SaferMatch(() => inst.MatchCall(type, name));
    public static bool SaferMatchCallvirt(this Instruction inst, MethodInfo method) => SaferMatch(() => inst.MatchCallvirt(method));
    public static bool SaferMatchCallvirt(this Instruction inst, Type type, string name) => SaferMatch(() => inst.MatchCallvirt(type, name));
    public static bool SaferMatch(Func<bool> cb) {
        try { return cb(); }
        catch (InvalidCastException) { return false; }
    }

    public static ILCursor GotoNextLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => cursor.GotoNextLoc(MoveType.Before, out value, predicate, def);
    public static ILCursor GotoNextLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (cursor.TryGotoNext(moveType, i => i.MatchStloc(out loc) && predicate(i) != false)) value = loc;
        else if (def == -1) throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        return cursor;
    }
    public static ILCursor GotoPrevLoc(this ILCursor cursor, out int value, Predicate<Instruction> predicate, int def = -1) => cursor.GotoPrevLoc(MoveType.Before, out value, predicate, def);
    public static ILCursor GotoPrevLoc(this ILCursor cursor, MoveType moveType, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (cursor.TryGotoPrev(moveType, i => i.MatchStloc(out loc) && predicate(i) != false)) value = loc;
        else if (def == -1) throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        return cursor;
    }

    public static ILCursor FindPrevLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (cursor.TryFindPrev(out ILCursor[] cs, i => i.MatchStloc(out loc) && predicate(i) != false)) value = loc;
        else if (def == -1) throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        c = cs[0];
        return cursor;
    }
    public static ILCursor FindNextLoc(this ILCursor cursor, out ILCursor c, out int value, Predicate<Instruction> predicate, int def = -1) {
        value = def;
        int loc = def;
        if (cursor.TryFindNext(out ILCursor[] cs, i => i.MatchStloc(out loc) && predicate(i) != false)) value = loc;
        else if (def == -1) throw new SymbolsNotFoundException("No Stloc with those conditions were found");
        if (def != -1 && value != def) ModContent.GetInstance<SpikysLib>().Logger.Warn($"Found loc {value} but default is {def}");
        c = cs[0];
        return cursor;
    }
}
