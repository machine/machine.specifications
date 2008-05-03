using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Machine.Core.ValueTypes
{
  public delegate Int32 CalculateHashCodeFunction(object a);

  public class CalculateHashCodeFunctionFactory : AbstractValueTypeInspector
  {
    public CalculateHashCodeFunction CreateCalculateHashCodeFunction<TType>()
    {
      return CreateCalculateHashCodeFunction(typeof(TType));
    }

    public CalculateHashCodeFunction CreateCalculateHashCodeFunction(Type type)
    {
      DynamicMethod method = new DynamicMethod("CreateCalculateHashCodeFunction_" + type.FullName, typeof(Int32), new Type[] { typeof(Object) }, type);
      ILGenerator il = method.GetILGenerator();

      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Castclass, type);
      IfNull(il, delegate() {
           il.Emit(OpCodes.Newobj, typeof(ArgumentNullException).GetConstructor(new Type[0]));
           il.Emit(OpCodes.Throw);
         }, delegate() { }
      );

      bool hasState = false;
      foreach (FieldInfo field in AllFields(type))
      {
        MethodInfo getHashCode = field.FieldType.GetMethod("GetHashCode", new Type[0]);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, type);
        if (field.FieldType.IsValueType && !field.FieldType.IsEnum)
        {
          il.Emit(OpCodes.Ldflda, field);
          il.Emit(OpCodes.Call, getHashCode);
        }
        else
        {
          il.Emit(OpCodes.Ldfld, field);
          IfNull(il, delegate() {
               il.Emit(OpCodes.Ldc_I4_0);
             }, delegate() {
               il.Emit(OpCodes.Ldarg_0);
               il.Emit(OpCodes.Castclass, type);
               il.Emit(OpCodes.Ldfld, field);
               il.Emit(OpCodes.Callvirt, getHashCode);
             }
          );
        }
        if (hasState)
        {
          il.Emit(OpCodes.Ldc_I4, 29);
          il.Emit(OpCodes.Mul);
          il.Emit(OpCodes.Add);
        }
        hasState = true;
      }
      if (!hasState)
      {
        il.Emit(OpCodes.Ldc_I4_0);
      }
      il.Emit(OpCodes.Ret);
      return (CalculateHashCodeFunction)method.CreateDelegate(typeof(CalculateHashCodeFunction));
    }
  }
}