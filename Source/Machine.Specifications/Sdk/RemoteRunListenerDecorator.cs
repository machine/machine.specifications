using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Xml.Linq;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Sdk
{
    using System;
    using System.Collections.Generic;

    public abstract class RemoteToInternalSpecificationRunListenerAdapter : MarshalByRefObject, ISpecificationRunListener
    {
        private readonly IMessageSink _listener;

        protected RemoteToInternalSpecificationRunListenerAdapter(object listener, string runOptionsXml)
        {
            _listener = (IMessageSink)listener;
            
            this.RunOptions = RunOptions.Parse(runOptionsXml);
            this.Runner = new DefaultRunner(this, RunOptions);
        }

        protected RunOptions RunOptions { get; private set; }

        protected ISpecificationRunner Runner { get; private set; }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblystart", assemblyInfo.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblyend", assemblyInfo.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnRunStart()
        {
            var root = new XElement("listener", new XElement("onrunstart"));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnRunEnd()
        {
            var root = new XElement("listener", new XElement("onrunend"));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnContextStart(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextstart", context.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnContextEnd(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextend", context.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var root = new XElement("listener", new XElement("onspecificationstart", specification.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var root = new XElement("listener", new XElement("onspecificationend", specification.ToXml(), result.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
        }

        public void OnFatalError(ExceptionResult exception)
        {
            var root = new XElement("listener", new XElement("onfatalerror", exception.ToXml()));

            this._listener.SyncProcessMessage(new RemoteListenerMessage(root));
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
                this.Properties = new Dictionary<string, string> { { "data", root.ToString() } };
            }

            public IDictionary Properties { get; private set; }
        }
    }
}