using System;

namespace Machine.Specifications.Runner.Impl
{
    internal class InvokeOnce
    {
        private readonly Action invocation;

        public InvokeOnce(Action invocation)
        {
            this.invocation = invocation;
        }

        public bool WasInvoked { get; private set; }

        public void Invoke()
        {
            if (WasInvoked)
            {
                return;
            }

            WasInvoked = true;

            invocation();
        }
    }
}
