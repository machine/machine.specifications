﻿using System;
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
            var currentContext = SynchronizationContext.Current;

            var context = new AsyncSynchronizationContext(currentContext);

            SynchronizationContext.SetSynchronizationContext(context);

            try
            {
                target.DynamicInvoke(args);

                var exception = context.WaitAsync().Result;

                if (exception != null)
                {
                    throw exception;
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(currentContext);
            }
        }
    }
}
