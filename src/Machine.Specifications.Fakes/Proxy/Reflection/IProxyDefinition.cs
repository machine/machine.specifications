using System;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public interface IProxyDefinition
    {
        Type Type { get; }

        object Create(IInterceptor interceptor, params object[] arguments);
    }
}
