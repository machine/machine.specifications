using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy
{
    public class Invocation : IInvocation
    {
        public Invocation(MethodInfo method, object[] arguments)
        {
            Method = method;
            Arguments = arguments;
        }

        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public object ReturnValue { get; set; }
    }
}
