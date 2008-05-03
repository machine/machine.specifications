using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Machine.Core.ValueTypes
{
  public delegate bool ObjectEqualityFunction(object a, object b);

  public class ObjectEqualityFunctionFactory : AbstractValueTypeInspector
  {
    public ObjectEqualityFunction CreateObjectEqualityFunction<TType>()
    {
      return CreateObjectEqualityFunction(typeof(TType));
    }

    public ObjectEqualityFunction CreateObjectEqualityFunction(Type type)
    {
      DynamicMethod method = NewMethod(type, typeof(bool), new Type[] { typeof(object), typeof(object) });
      ILGenerator il = method.GetILGenerator();
      MethodInfo objectEquals = typeof(Object).GetMethod("Equals", new Type[] { typeof(Object), typeof(Object) });
      bool hasState = false;
      foreach (FieldInfo field in AllFields(type))
      {
        Label label = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, type);
        if (field.FieldType.IsValueType && !field.FieldType.IsEnum)
        {
          MethodInfo valueEquals = field.FieldType.GetMethod("Equals", new Type[] { field.FieldType });
          il.Emit(OpCodes.Ldflda, field);
          il.Emit(OpCodes.Ldarg_1);
          il.Emit(OpCodes.Castclass, type);
          il.Emit(OpCodes.Ldfld, field);
          il.Emit(OpCodes.Callvirt, valueEquals);
        }
        else
        {
          il.Emit(OpCodes.Ldfld, field);
          if (field.FieldType.IsValueType)
          {
            il.Emit(OpCodes.Box, field.FieldType);
          }
          il.Emit(OpCodes.Ldarg_1);
          il.Emit(OpCodes.Castclass, type);
          il.Emit(OpCodes.Ldfld, field);
          if (field.FieldType.IsValueType)
          {
            il.Emit(OpCodes.Box, field.FieldType);
          }
          il.Emit(OpCodes.Call, objectEquals);
        }
        il.Emit(OpCodes.Brtrue, label);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(label);
        hasState = true;
      }
      if (hasState)
      {
        il.Emit(OpCodes.Ldc_I4_1);
      }
      else
      {
        il.Emit(OpCodes.Ldc_I4_0);
      }
      il.Emit(OpCodes.Ret);
      return (ObjectEqualityFunction)method.CreateDelegate(typeof(ObjectEqualityFunction));
    }
  }
}