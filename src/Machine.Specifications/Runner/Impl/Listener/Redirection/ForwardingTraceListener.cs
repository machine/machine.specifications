using System.Diagnostics;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class ForwardingTraceListener : TraceListener
    {
        readonly TraceListener[] _inner;

        public ForwardingTraceListener(TraceListener[] inner)
        {
            _inner = inner;
        }

        public override void Fail(string message, string detailMessage)
        {
            _inner.Each(x => x.Fail(message, detailMessage));
        }

        public override void WriteLine(string message)
        {
            _inner.Each(x => x.WriteLine(message));
        }

        public override void Write(string message)
        {
            _inner.Each(x => x.Write(message));
        }
    }
}