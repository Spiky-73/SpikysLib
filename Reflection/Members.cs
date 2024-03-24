using System;
using System.Reflection;

namespace SpikysLib.Reflection;

public abstract class Member<TMemberInfo> where TMemberInfo : MemberInfo {
    internal Member(TMemberInfo? info) {
        if (info is null) throw new ArgumentNullException(nameof(info));
        ValidateMemberInfo(info);
        MemberInfo = info;
    }

    public Type DeclaringType => MemberInfo.DeclaringType!;
    public string Name => MemberInfo.Name;
    public TMemberInfo MemberInfo { get; }

    protected abstract void ValidateMemberInfo(TMemberInfo info);

    public static implicit operator TMemberInfo(Member<TMemberInfo> member) => member.MemberInfo;

    public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}

public sealed class Field<TObject, T> : Member<FieldInfo> {
    public Field(FieldInfo? info) : base(info) { }
    public Field(string name) : this(typeof(TObject).GetField(name, InstanceFlags)) { }

    public T GetValue(TObject self) => (T)MemberInfo.GetValue(self)!;
    public void SetValue(TObject self, T value) => MemberInfo.SetValue(self, value);

    protected override void ValidateMemberInfo(FieldInfo info) {
        if (info.FieldType != typeof(T)) throw new MissingFieldException(info.DeclaringType!.FullName, info.Name);
    }
}

public sealed class StaticField<T> : Member<FieldInfo> {

    public StaticField(FieldInfo? info) : base(info) { }
    public StaticField(Type type, string name) : this(type.GetField(name, StaticFlags)) { }

    public T GetValue() => (T)MemberInfo.GetValue(null)!;
    public void SetValue(T value) => MemberInfo.SetValue(null, value);

    protected override void ValidateMemberInfo(FieldInfo info) {
        if (info.FieldType != typeof(T)) throw new MissingFieldException(info.DeclaringType!.FullName, info.Name);
    }
}

public sealed class Property<TObject, T> : Member<PropertyInfo> {
    public Property(PropertyInfo? info) : base(info) { }
    public Property(string name) : this(typeof(TObject).GetProperty(name, InstanceFlags)) { }

    public T GetValue(TObject self) => (T)MemberInfo.GetValue(self)!;
    public void SetValue(TObject self, T value) => MemberInfo.SetValue(self, value);

    public Method<TObject, T>? GetMethod => MemberInfo.CanRead ? new(MemberInfo.GetMethod!) : null;
    public Method<TObject, object?>? SetMethod => MemberInfo.CanWrite ? new(MemberInfo.SetMethod!) : null;

    protected sealed override void ValidateMemberInfo(PropertyInfo info) {
        if (info.PropertyType != typeof(T)) throw new MissingFieldException(info.DeclaringType!.FullName, info.Name);
    }
}

public sealed class StaticProperty<T> : Member<PropertyInfo> {
    public StaticProperty(PropertyInfo? info) : base(info) { }
    public StaticProperty(Type type, string name) : this(type.GetProperty(name, StaticFlags)) { }

    public T GetValue() => (T)MemberInfo.GetValue(null)!;
    public void SetValue(T value) => MemberInfo.SetValue(null, value);

    public StaticMethod<T>? GetMethod => MemberInfo.CanRead ? new(MemberInfo.GetMethod!) : null;
    public StaticMethod<object?>? SetMethod => MemberInfo.CanWrite ? new(MemberInfo.SetMethod!) : null;

    protected sealed override void ValidateMemberInfo(PropertyInfo info) {
        if (info.PropertyType != typeof(T)) throw new MissingFieldException(info.DeclaringType!.FullName, info.Name);
    }
}

public sealed class Method<TObject, T> : Member<MethodInfo> {
    public Method(MethodInfo? info) : base(info) { }
    public Method(string name, params Type[] argsType) : this(typeof(TObject).GetMethod(name, InstanceFlags, argsType)) { }

    public T Invoke(TObject self, params object[] args) => (T)MemberInfo.Invoke(self, args)!;

    protected sealed override void ValidateMemberInfo(MethodInfo info) {
        if ((typeof(T) != typeof(object) || info.ReturnType != typeof(void)) && info.ReturnType != typeof(T)) throw new MissingMethodException(info.DeclaringType!.FullName, info.Name);
    }
}

public sealed class StaticMethod<T> : Member<MethodInfo> {
    public StaticMethod(MethodInfo? info) : base(info) { }
    public StaticMethod(Type type, string name, params Type[] argsType) : this(type.GetMethod(name, StaticFlags, argsType)) { }

    public T Invoke(params object[] args) => (T)MemberInfo.Invoke(null, args)!;

    protected sealed override void ValidateMemberInfo(MethodInfo info) {
        if ((typeof(T) != typeof(object) || info.ReturnType != typeof(void)) && info.ReturnType != typeof(T)) throw new MissingMethodException(info.DeclaringType!.FullName, info.Name);
    }
}