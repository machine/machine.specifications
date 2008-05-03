using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Machine.Core.ValueTypes
{
  public abstract class AbstractValueTypeInspector
  {
    public IEnumerable<FieldInfo> AllFields(Type type)
    {
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
      {
        yield return field;
      }
    }

    public delegate void Emitter();

    public void IfNull(ILGenerator il, Emitter isTrue, Emitter isFalse)
    {
      Label nope = il.DefineLabel();
      Label done = il.DefineLabel();
      il.Emit(OpCodes.Ldnull);
      il.Emit(OpCodes.Ceq); // Has 1 if NULL
      il.Emit(OpCodes.Ldc_I4_0);
      il.Emit(OpCodes.Ceq); // Has 1 if NOT NULL
      il.Emit(OpCodes.Brtrue, nope);
      isTrue();
      il.Emit(OpCodes.Br, done);
      il.MarkLabel(nope);
      isFalse();
      il.MarkLabel(done);
    }

    public DynamicMethod NewMethod(Type owner, Type returnType, Type[] parameterTypes)
    {
      return new DynamicMethod(GetType().Name + "_" + owner.FullName, returnType, parameterTypes, owner);
    }

    public void ThrowIfArg0CastFailsOrNull(ILGenerator il, Type type)
    {
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Castclass, type);
      IfNull(il, delegate() {
           il.Emit(OpCodes.Newobj, typeof(ArgumentNullException).GetConstructor(new Type[0]));
           il.Emit(OpCodes.Throw);
         }, delegate() { }
      );
    }
  }
}