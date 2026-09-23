#if !NETSTANDARD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Impl
{
    internal class RemoteRunListenerDecorator : MarshalByRefObject, ISpecificationRunListener
    {
        private readonly IMessageSink listener;

        public RemoteRunListenerDecorator(object listener)
        {
            this.listener = (IMessageSink)listener;
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblystart", assemblyInfo.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblyend", assemblyInfo.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnRunStart()
        {
            var root = new XElement("listener", new XElement("onrunstart"));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnRunEnd()
        {
            var root = new XElement("listener", new XElement("onrunend"));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnContextStart(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextstart", context.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnContextEnd(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextend", context.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var root = new XElement("listener", new XElement("onspecificationstart", specification.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var root = new XElement("listener", new XElement("onspecificationend", specification.ToXml(), result.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnFatalError(ExceptionResult exception)
        {
            var root = new XElement("listener", new XElement("onfatalerror", exception.ToXml()));

            listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private class RemoteListenerMessage : MarshalByRefObject, IMessage
        {
            public RemoteListenerMessage(XElement root)
            {
                Properties = new Dictionary<string, string>
                {
                    {"data", root.ToString()}
                };
            }

            public IDictionary Properties { get; }
        }
    }
}
#endif
