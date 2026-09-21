using System;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Fakes.Proxy;
using Machine.Specifications.Fakes.Proxy.Reflection;

namespace Machine.Specifications.Fakes.Specs
{
    public interface IRob
    {
        object Execute(int arg1);
    }

    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = null;
        }
    }

    class TestSpec
    {
        static TypeEmitterFactory factory;

        static MethodInfo execute_method;

        static Type type;

        static Interceptor interceptor;

        Establish context = () =>
        {
            factory = new TypeEmitterFactory();
            interceptor = new Interceptor();
            execute_method = typeof(IRob).GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);
        };

        Because of = () =>
        {
            var emitter = factory.DefineType(typeof(object));

            emitter.EmitInterface(typeof(IRob));
            emitter.EmitConstructor(typeof(object).GetConstructors().First());
            emitter.EmitMethod(execute_method);

            type = emitter.CreateType();
        };

        It should = () =>
        {
            var value = Activator.CreateInstance(type, interceptor);

            var ret = execute_method.Invoke(value, new object[] {4});

            int r = 1;
        };
    }
}
