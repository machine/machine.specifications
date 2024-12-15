using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public class TypeEmitterFactory : ITypeEmitterFactory
    {
        private const string Name = "Machine.Specifications.Fakes.Proxy";

        private const string ModuleName = Name + ".dll";

        private readonly ModuleBuilder moduleBuilder;

        private int count;

        public TypeEmitterFactory()
        {
            var name = new AssemblyName(Name);

            moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(name, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(ModuleName);
        }

        public ITypeEmitter DefineType(Type type)
        {
            var value = Interlocked.Increment(ref count);

            var typeBuilder = moduleBuilder.DefineType(type.Name + $"Proxy_{value}", TypeAttributes.Class);

            return new TypeEmitter(typeBuilder);
        }
    }
}
