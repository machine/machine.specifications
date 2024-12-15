using System;

namespace Machine.Specifications.Fakes.Proxy
{
    public interface IProxyFactory
    {
        object CreateProxy(Type type, IInterceptor interceptor, params object[] arguments);
    }
}
