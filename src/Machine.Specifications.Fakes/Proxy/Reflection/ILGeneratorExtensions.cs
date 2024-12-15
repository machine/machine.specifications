using System;
using System.Reflection.Emit;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public static class ILGeneratorExtensions
    {
        public static void EmitBox(this ILGenerator generator, Type type)
        {
            if (type.IsValueType || type.IsGenericParameter)
                generator.Emit(OpCodes.Box, type);
        }

        public static void EmitLoadArgument(this ILGenerator generator, LocalBuilder variable)
        {
            generator.Emit(OpCodes.Ldarg, variable);
        }

        public static void EmitLoadValue(this ILGenerator generator, int value)
        {
            generator.Emit(OpCodes.Ldc_I4, value);
        }

        public static LocalBuilder EmitNewArray(this ILGenerator generator, Type elementType, int size)
        {
            var variable = generator.DeclareLocal(elementType.MakeArrayType());

            generator.EmitLoadValue(size);
            generator.Emit(OpCodes.Newarr, elementType);
            generator.Emit(OpCodes.Stloc, variable);

            return variable;
        }
    }
}
