using System;
using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public interface ITypeEmitter
    {
        void EmitConstructor(ConstructorInfo constructor);

        void EmitInterface(Type type);

        void EmitMethod(MethodInfo method);

        Type CreateType();
    }
}
