using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Machine.Core.ValueTypes
{
  public delegate string ToStringFunction(object a);

  public class ToStringFunctionFactory : AbstractValueTypeInspector
  {
    public ToStringFunction CreateToStringFunction<TType>()
    {
      return CreateToStringFunction(typeof(TType));
    }

    public ToStringFunction CreateToStringFunction(Type type)
    {
      DynamicMethod method = NewMethod(type, typeof(string), new Type[] { typeof(object) });
      ILGenerator il = method.GetILGenerator();
      LocalBuilder sbLocal = il.DeclareLocal(typeof(StringBuilder));

      ThrowIfArg0CastFailsOrNull(il, type);

      MethodInfo appendString = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) });
      MethodInfo appendObject = typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(object) });

      il.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(new Type[0]));
      il.Emit(OpCodes.Stloc, sbLocal);
      il.Emit(OpCodes.Ldloc, sbLocal);
      il.Emit(OpCodes.Ldstr, type.Name + "<");
      il.Emit(OpCodes.Callvirt, appendString);

      bool hasState = false;
      foreach (FieldInfo field in AllFields(type))
      {
        if (hasState)
        {
          il.Emit(OpCodes.Ldloc, sbLocal);
          il.Emit(OpCodes.Ldstr, ", ");
          il.Emit(OpCodes.Callvirt, appendString);
        }
        il.Emit(OpCodes.Ldloc, sbLocal);
        il.Emit(OpCodes.Ldstr, MakePrettyName(field.Name) + "=");
        il.Emit(OpCodes.Callvirt, appendString);
        il.Emit(OpCodes.Ldloc, sbLocal);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, type);
        if (field.FieldType.IsValueType)
        {
          il.Emit(OpCodes.Ldfld, field);
          il.Emit(OpCodes.Box, field.FieldType);
        }
        else
        {
          il.Emit(OpCodes.Ldfld, field);
        }
        il.Emit(OpCodes.Callvirt, appendObject);
        hasState = true;
      }
      il.Emit(OpCodes.Ldloc, sbLocal);
      il.Emit(OpCodes.Ldstr, ">");
      il.Emit(OpCodes.Callvirt, appendString);
      il.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("ToString", new Type[0]));
      il.Emit(OpCodes.Ret);
      return (ToStringFunction)method.CreateDelegate(typeof(ToStringFunction));
    }

    private string MakePrettyName(string name)
    {
      StringBuilder sb = new StringBuilder();
      bool uppercaseNext = true;
      for (int i = 0; i < name.Length; ++i)
      {
        if (name[i] == '_')
        {
          uppercaseNext = true;
          continue;
        }
        if (uppercaseNext)
        {
          sb.Append(Char.ToUpper(name[i]));
        }
        else
        {
          sb.Append(name[i]);
        }
        uppercaseNext = false;
      }
      return sb.ToString();
    }
  }
}
