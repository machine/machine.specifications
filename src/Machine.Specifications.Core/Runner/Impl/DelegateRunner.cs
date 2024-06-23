using System;
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

                var exceptionDispatchInfo = context.WaitAsync().Result;

                exceptionDispatchInfo?.Throw();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
        }

        private bool IsAsyncVoid()
        {
            return target.Method.ReturnType == typeof(void) &&
                   target.Method.GetCustomAttribute<AsyncStateMachineAttribute>() != null;
        }
    }
}
