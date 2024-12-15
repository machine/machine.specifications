using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy
{
    public interface IInvocation
    {
        MethodInfo Method { get; }

        object[] Arguments { get; }

        object ReturnValue { get; set; }
    }
}
