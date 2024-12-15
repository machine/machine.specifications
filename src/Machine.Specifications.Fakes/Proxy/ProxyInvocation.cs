using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy
{
    public abstract class ProxyInvocation : IInvocation
    {
        protected ProxyInvocation(MethodInfo method, object[] arguments)
        {
            Method = method;
            Arguments = arguments;
        }

        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public object ReturnValue { get; set; }
    }
}
