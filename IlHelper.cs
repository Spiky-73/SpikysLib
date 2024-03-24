using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace SpikysLib;

public static class IlHelper {

    public static bool SaferMatchCall(this Instruction inst, MethodInfo method) {
        try {
            return inst.MatchCall(method);
        }
        catch (InvalidCastException) {
            return false;
        }
    }
    public static bool SaferMatchCall(this Instruction inst, Type type, string name) {
        try {
            return inst.MatchCall(type, name);
        }
        catch (InvalidCastException) {
            return false;
        }
    }

}
