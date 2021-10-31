using System.Diagnostics;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class ForwardingTraceListener : TraceListener
    {
        private readonly TraceListener[] inner;

        public ForwardingTraceListener(TraceListener[] inner)
        {
            this.inner = inner;
        }

        public override void Fail(string message, string detailMessage)
        {
            inner.Each(x => x.Fail(message, detailMessage));
        }

        public override void WriteLine(string message)
        {
            inner.Each(x => x.WriteLine(message));
        }

        public override void Write(string message)
        {
            inner.Each(x => x.Write(message));
        }
    }
}
