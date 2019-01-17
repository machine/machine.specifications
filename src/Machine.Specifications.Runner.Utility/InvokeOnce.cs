using System;

namespace Machine.Specifications.Runner.Utility
{
    internal class InvokeOnce
    {
        readonly Action _invocation;
        public bool WasInvoked { get; private set; }

        public InvokeOnce(Action invocation)
        {
            _invocation = invocation;
        }

        public void Invoke()
        {
            if (WasInvoked)
            {
                return;
            }

            WasInvoked = true;
            _invocation();
        }
    }
}