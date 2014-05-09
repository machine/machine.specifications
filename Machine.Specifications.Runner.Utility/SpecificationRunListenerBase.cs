using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// The specification run listener base is a base class which takes the burden to implement IMessageSink and translates
    /// information about specification execution over app domain boundaries.
    /// </summary>
    [Serializable]
    public class SpecificationRunListenerBase : MarshalByRefObject, ISpecificationRunListener, IMessageSink
    {
        public virtual void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
        }

        public virtual void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
        }

        public virtual void OnRunStart()
        {
        }

        public virtual void OnRunEnd()
        {
        }

        public virtual void OnContextStart(ContextInfo contextInfo)
        {
        }

        public virtual void OnContextEnd(ContextInfo contextInfo)
        {
        }

        public virtual void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
        }

        public virtual void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
        }

        public virtual void OnFatalError(ExceptionResult exceptionResult)
        {
        }

        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

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
    }
}
