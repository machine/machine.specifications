#if !NET6_0_OR_GREATER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Xml.Linq;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Sdk
{
    public abstract class RemoteToInternalSpecificationRunListenerAdapter : MarshalByRefObject, ISpecificationRunListener
    {
        private readonly IMessageSink listener;

        protected RemoteToInternalSpecificationRunListenerAdapter(object listener, string runOptionsXml)
        {
            this.listener = (IMessageSink)listener;

            RunOptions = RunOptions.Parse(runOptionsXml);
            Runner = new DefaultRunner(this, RunOptions);
        }

        protected RunOptions RunOptions { get; }

        protected ISpecificationRunner Runner { get; }

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
                    { "data", root.ToString() }
                };
            }

            public IDictionary Properties { get; }
        }
    }
}

#endif
