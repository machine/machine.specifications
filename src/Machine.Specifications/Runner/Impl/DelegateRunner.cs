using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Machine.Specifications.Runner.Impl
{
    internal class DelegateRunner
    {
        private readonly Delegate target;

        private readonly object[] args;

        public DelegateRunner(Delegate target, params object[] args)
        {
            this.target = target;
            this.args = args;
        }

        public void Execute()
        {
#if NET35
            target.DynamicInvoke(args);
#else
            if (!IsAsyncVoid())
            {
                target.DynamicInvoke(args);

                return;
            }

            var currentContext = SynchronizationContext.Current;

            var context = new AsyncSynchronizationContext(currentContext);

            SynchronizationContext.SetSynchronizationContext(context);

            try
            {
                target.DynamicInvoke(args);

                var exception = context.WaitAsync();

                if (exception != null)
                {
                    throw exception;
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
#endif
        }

        private bool IsAsyncVoid()
        {
#if !NETSTANDARD
            var method = target.Method;
#else
            var method = target.GetMethodInfo();
#endif

#if !NET35 && !NET40
            return method.ReturnType == typeof(void) &&
                   method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
#else
            return false;
#endif
        }
    }
}
