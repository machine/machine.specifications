using System;
using System.Collections.Concurrent;
using Machine.Specifications.Fakes.Proxy.Reflection;

namespace Machine.Specifications.Fakes.Proxy
{
    public class ProxyFactory : IProxyFactory
    {
        private readonly ITypeEmitterFactory typeEmitterFactory;

        private readonly ConcurrentDictionary<Type, Lazy<IProxyDefinition>> proxies =
            new ConcurrentDictionary<Type, Lazy<IProxyDefinition>>();

        public ProxyFactory(ITypeEmitterFactory typeEmitterFactory)
        {
            this.typeEmitterFactory = typeEmitterFactory;
        }

        public object CreateProxy(Type type, IInterceptor interceptor, params object[] arguments)
        {
            var proxy = proxies.GetOrAdd(type, CreateProxy);

            return proxy.Value.Create(interceptor, arguments);
        }

        private Lazy<IProxyDefinition> CreateProxy(Type type)
        {
            return new Lazy<IProxyDefinition>(() => GetProxyDefinition(type));
        }

        private IProxyDefinition GetProxyDefinition(Type type)
        {
            var emitter = typeEmitterFactory.DefineType(type);

            var visitor = GetProxyVisitor(type, emitter);
            visitor.Visit(type);

            return new ProxyDefinition(emitter.CreateType());
        }

        private ProxyVisitor GetProxyVisitor(Type type, ITypeEmitter emitter)
        {
            if (type.IsInterface)
            {
                return new InterfaceProxyVisitor(type);
            }

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                return new DelegateProxyVisitor();
            }

            return new ClassProxyVisitor(emitter);
        }
    }
}
