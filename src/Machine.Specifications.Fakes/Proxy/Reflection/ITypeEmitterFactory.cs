using System;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public interface ITypeEmitterFactory
    {
        ITypeEmitter DefineType(Type type);
    }
}
