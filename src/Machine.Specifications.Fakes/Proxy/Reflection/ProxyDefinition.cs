using System;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public class ProxyDefinition : IProxyDefinition
    {
        public ProxyDefinition(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public object Create(IInterceptor interceptor, params object[] arguments)
        {
            return Activator.CreateInstance(Type, arguments);
        }
    }
}
