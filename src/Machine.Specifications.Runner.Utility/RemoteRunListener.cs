using System;
using System.IO;
#if !NETSTANDARD
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// The remote run listener is a decorator class which takes the burden to implement IMessageSink and translates
    /// information about specification execution over app domain boundaries.
    /// </summary>
#if !NETSTANDARD
    [Serializable]
#endif
    internal class RemoteRunListener : LongLivedMarshalByRefObject, ISpecificationRunListener
#if !NETSTANDARD
        , IMessageSink
#endif
    {
        private readonly ISpecificationRunListener _runListener;

        public RemoteRunListener(ISpecificationRunListener runListener)
        {
            _runListener = runListener;
        }

        public virtual void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            _runListener.OnAssemblyStart(assemblyInfo);
        }

        public virtual void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            _runListener.OnAssemblyEnd(assemblyInfo);
        }

        public virtual void OnRunStart()
        {
            _runListener.OnRunStart();
        }

        public virtual void OnRunEnd()
        {
            _runListener.OnRunEnd();
        }

        public virtual void OnContextStart(ContextInfo contextInfo)
        {
            _runListener.OnContextStart(contextInfo);
        }

        public virtual void OnContextEnd(ContextInfo contextInfo)
        {
            _runListener.OnContextEnd(contextInfo);
        }

        public virtual void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
            _runListener.OnSpecificationStart(specificationInfo);
        }

        public virtual void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
            _runListener.OnSpecificationEnd(specificationInfo, result);
        }

        public virtual void OnFatalError(ExceptionResult exceptionResult)
        {
            _runListener.OnFatalError(exceptionResult);
        }

#if !NETSTANDARD
        public IMessage SyncProcessMessage(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            if (methodCall != null)
            {
                return RemotingServices.ExecuteMessage(this, methodCall);
            }

            // This is all a bit ugly but gives us version independance for the moment
            string value = msg.Properties["data"] as string;
            var doc = XDocument.Load(new StringReader(value));
            var element = doc.XPathSelectElement("/listener/*");
            var listener = (ISpecificationRunListener) this;

            switch (element.Name.ToString())
            {
                case "onassemblystart":
                    listener.OnAssemblyStart(AssemblyInfo.Parse(element.XPathSelectElement("//onassemblystart/*").ToString()));
                    break;
                case "onassemblyend":
                    listener.OnAssemblyEnd(AssemblyInfo.Parse(element.XPathSelectElement("//onassemblyend/*").ToString()));
                    break;
                case "onrunstart":
                    listener.OnRunStart();
                    break;
                case "onrunend":
                    listener.OnRunEnd();
                    break;
                case "oncontextstart":
                    listener.OnContextStart(ContextInfo.Parse(element.XPathSelectElement("//oncontextstart/*").ToString()));
                    break;
                case "oncontextend":
                    listener.OnContextEnd(ContextInfo.Parse(element.XPathSelectElement("//oncontextend/*").ToString()));
                    break;
                case "onspecificationstart":
                    listener.OnSpecificationStart(SpecificationInfo.Parse(element.XPathSelectElement("//onspecificationstart/*").ToString()));
                    break;
                case "onspecificationend":
                    listener.OnSpecificationEnd(
                        SpecificationInfo.Parse(element.XPathSelectElement("//onspecificationend/specificationinfo").ToString()), 
                        Result.Parse(element.XPathSelectElement("//onspecificationend/result").ToString()));
                    break;
                case "onfatalerror":
                    listener.OnFatalError(ExceptionResult.Parse(element.XPathSelectElement("//onfatalerror/*").ToString()));
                    break;
            }

            return null;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }

        public IMessageSink NextSink { get; private set; }
#endif
    }
}
