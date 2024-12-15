using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public class TypeEmitter : ITypeEmitter
    {
        private static readonly MethodInfo GetterMethod = typeof(MethodBase).GetMethod(
            "GetMethodFromHandle",
            new[] {typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle)});

        private static readonly MethodInfo InterceptMethod = typeof(IInterceptor).GetMethod(
            "Intercept",
            new[] {typeof(IInvocation)});

        private static readonly ConstructorInfo InvocationConstructor = typeof(Invocation).GetConstructor(
            new[] {typeof(MethodInfo), typeof(object[])});

        private static readonly MethodInfo ReturnProperty = typeof(Invocation).GetMethod("get_ReturnValue");

        private readonly TypeBuilder typeBuilder;

        private readonly FieldInfo interceptorField;

        private readonly List<(MethodInfo, FieldBuilder)> fields = new List<(MethodInfo, FieldBuilder)>();

        public TypeEmitter(TypeBuilder typeBuilder)
        {
            this.typeBuilder = typeBuilder;

            interceptorField = typeBuilder.DefineField(
                "interceptor",
                typeof(IInterceptor),
                FieldAttributes.Private);
        }

        public void EmitConstructor(ConstructorInfo constructor)
        {
            var constructorParameters = constructor
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var parameters = new List<Type> { typeof(IInterceptor)};
            parameters.AddRange(constructorParameters);

            var builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameters.ToArray());

            var generator = builder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            for (var i = 0; i < constructorParameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg, i + 2);
            }

            generator.Emit(OpCodes.Call, constructor);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, interceptorField);

            generator.Emit(OpCodes.Ret);
        }

        public void EmitInterface(Type type)
        {
            typeBuilder.AddInterfaceImplementation(type);
        }

        public void EmitMethod(MethodInfo method)
        {
            var parameters = method
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var methodField = typeBuilder.DefineField(
                method.Name + "Field",
                typeof(MethodInfo),
                FieldAttributes.Static);

            var builder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.CallingConvention,
                method.ReturnType,
                parameters);

            var generator = builder.GetILGenerator();

            // Create arguments array
            var arguments = generator.DeclareLocal(typeof(object).MakeArrayType());

            generator.Emit(OpCodes.Ldc_I4, parameters.Length);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc, arguments);

            // Set method arguments into arguments array
            for (var i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldloc, arguments);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldarg, i + 1);

                if (parameters[i].IsValueType || parameters[i].IsGenericParameter)
                {
                    generator.Emit(OpCodes.Box, parameters[i]);
                }

                generator.Emit(OpCodes.Stelem_Ref);
            }

            // Create new invocation object
            var invocation = generator.DeclareLocal(typeof(Invocation));

            generator.Emit(OpCodes.Ldsfld, methodField);
            generator.Emit(OpCodes.Ldloc, arguments);
            generator.Emit(OpCodes.Newobj, InvocationConstructor);
            generator.Emit(OpCodes.Stloc, invocation);

            // Call intercept method
            generator.BeginExceptionBlock();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, interceptorField);
            generator.Emit(OpCodes.Ldloc, invocation);
            generator.Emit(OpCodes.Callvirt, InterceptMethod);
            generator.BeginFinallyBlock();
            // todo: set out/ref arguments
            generator.EndExceptionBlock();

            if (method.ReturnType != typeof(void))
            {
                generator.Emit(OpCodes.Ldloc, invocation);
                generator.Emit(OpCodes.Callvirt, ReturnProperty);

                if (method.ReturnType.IsValueType)
                {
                    generator.Emit(OpCodes.Unbox_Any, method.ReturnType);
                }
                else
                {
                    generator.Emit(OpCodes.Castclass, method.ReturnType);
                }
            }

            generator.Emit(OpCodes.Ret);

            fields.Add((method, methodField));
        }

        public Type CreateType()
        {
            CreateStaticConstructor();

            return typeBuilder.CreateTypeInfo();
        }

        private void CreateStaticConstructor()
        {
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Static,
                CallingConventions.Standard,
                Type.EmptyTypes);

            var generator = constructor.GetILGenerator();

            foreach (var (method, field) in fields)
            {
                generator.Emit(OpCodes.Ldtoken, method);
                generator.Emit(OpCodes.Ldtoken, method.DeclaringType);
                generator.Emit(OpCodes.Call, GetterMethod);
                generator.Emit(OpCodes.Castclass, typeof(MethodInfo));
                generator.Emit(OpCodes.Stsfld, field);
            }

            generator.Emit(OpCodes.Ret);
        }
    }
}
