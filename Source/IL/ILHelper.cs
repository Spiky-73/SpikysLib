using System;
using System.Linq.Expressions;
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
        try { return cb(); } catch (InvalidCastException) { return false; }
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

    public static ILCursor Emit(this ILCursor cursor, OpCode opcode, LambdaExpression expr) => CallFieldOrMethod(expr, i => cursor.Emit(opcode, i), i => cursor.Emit(opcode, i));
    public static ILCursor EmitCall(this ILCursor cursor, LambdaExpression expr) => cursor.EmitCall(TypeHelper.GetMethod(expr));
    public static ILCursor EmitCallvirt(this ILCursor cursor, LambdaExpression expr) => cursor.EmitCallvirt(TypeHelper.GetMethod(expr));
    public static ILCursor EmitJmp(this ILCursor cursor, LambdaExpression expr) => cursor.EmitJmp(TypeHelper.GetMethod(expr));
    public static ILCursor EmitLdfld(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdfld(TypeHelper.GetField(expr));
    public static ILCursor EmitLdflda(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdflda(TypeHelper.GetField(expr));
    public static ILCursor EmitLdftn(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdftn(TypeHelper.GetMethod(expr));
    public static ILCursor EmitLdsfld(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdsfld(TypeHelper.GetField(expr));
    public static ILCursor EmitLdsflda(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdsflda(TypeHelper.GetField(expr));
    public static ILCursor EmitLdtoken(this ILCursor cursor, LambdaExpression expr) => CallFieldOrMethod(expr, cursor.EmitLdtoken, cursor.EmitLdtoken);
    public static ILCursor EmitLdvirtftn(this ILCursor cursor, LambdaExpression expr) => cursor.EmitLdvirtftn(TypeHelper.GetMethod(expr));
    public static ILCursor EmitNewobj(this ILCursor cursor, LambdaExpression expr) => cursor.EmitNewobj(TypeHelper.GetConstructor(expr));
    public static ILCursor EmitStfld(this ILCursor cursor, LambdaExpression expr) => cursor.EmitStfld(TypeHelper.GetField(expr));
    public static ILCursor EmitStsfld(this ILCursor cursor, LambdaExpression expr) => cursor.EmitStsfld(TypeHelper.GetField(expr));

    public static bool MatchCall(this Instruction instr, LambdaExpression expr) => instr.MatchCall(TypeHelper.GetMethod(expr));
    public static bool MatchCallvirt(this Instruction instr, LambdaExpression expr) => instr.MatchCallvirt(TypeHelper.GetMethod(expr));
    public static bool MatchCallOrCallvirt(this Instruction instr, LambdaExpression expr) => instr.MatchCallOrCallvirt(TypeHelper.GetMethod(expr));
    public static bool MatchJmp(this Instruction instr, LambdaExpression expr) => instr.MatchJmp(TypeHelper.GetMethod(expr));
    public static bool MatchLdfld(this Instruction instr, LambdaExpression expr) => instr.MatchLdfld(TypeHelper.GetField(expr));
    public static bool MatchLdflda(this Instruction instr, LambdaExpression expr) => instr.MatchLdflda(TypeHelper.GetField(expr));
    public static bool MatchLdftn(this Instruction instr, LambdaExpression expr) => instr.MatchLdftn(TypeHelper.GetMethod(expr));
    public static bool MatchLdsfld(this Instruction instr, LambdaExpression expr) => instr.MatchLdsfld(TypeHelper.GetField(expr));
    public static bool MatchLdsflda(this Instruction instr, LambdaExpression expr) => instr.MatchLdsflda(TypeHelper.GetField(expr));
    public static bool MatchLdToken(this Instruction instr, LambdaExpression expr) => CallFieldOrMethod(expr, instr.MatchLdtoken, instr.MatchLdtoken);
    public static bool MatchLdvirtftn(this Instruction instr, LambdaExpression expr) => instr.MatchLdvirtftn(TypeHelper.GetMethod(expr));
    public static bool MatchNewobj(this Instruction instr, LambdaExpression expr) => instr.MatchNewobj(TypeHelper.GetConstructor(expr));
    public static bool MatchStfld(this Instruction instr, LambdaExpression expr) => instr.MatchStfld(TypeHelper.GetField(expr));
    public static bool MatchStsfld(this Instruction instr, LambdaExpression expr) => instr.MatchStfld(TypeHelper.GetField(expr));

    private static T CallFieldOrMethod<T>(LambdaExpression expr, Func<FieldInfo, T> field, Func<MethodBase, T> method) => TypeHelper.GetMember(expr) switch {
        FieldInfo f => field(f),
        MethodBase m => method(m),
        _ => throw new ArgumentException("expr must be a field of a method"),
    };
}
