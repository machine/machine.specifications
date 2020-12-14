#if !NETSTANDARD
using System.Runtime.Remoting.Messaging;

namespace Machine.Specifications.Runner.Utility
{
    internal class NullMessageSink : IMessageSink
    {
        public IMessage SyncProcessMessage(IMessage msg)
        {
            return msg;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }

        public IMessageSink NextSink { get; private set; }
    }
}
#endif
