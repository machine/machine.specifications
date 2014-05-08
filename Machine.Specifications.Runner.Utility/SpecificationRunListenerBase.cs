using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class SpecificationRunListenerBase : MarshalByRefObject, ISpecificationRunListener, IMessageSink
    {
        protected virtual void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
        }

        protected virtual void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
        }

        protected virtual void OnRunStart()
        {
        }

        protected virtual void OnRunEnd()
        {
        }

        protected virtual void OnContextStart(ContextInfo contextInfo)
        {
        }

        protected virtual void OnContextEnd(ContextInfo contextInfo)
        {
        }

        protected virtual void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
        }

        protected virtual void OnSpecificationEnd(SpecificationInfo specificationInfo, Result result)
        {
        }

        protected virtual void OnFatalError(ExceptionResult exceptionResult)
        {
        }

        void ISpecificationRunListener.OnAssemblyStart(string assemblyInfoXml)
        {
            this.OnAssemblyStart(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void ISpecificationRunListener.OnAssemblyEnd(string assemblyInfoXml)
        {
            this.OnAssemblyEnd(AssemblyInfo.Parse(assemblyInfoXml));
        }

        void ISpecificationRunListener.OnRunStart()
        {
            this.OnRunStart();
        }

        void ISpecificationRunListener.OnRunEnd()
        {
            this.OnRunEnd();
        }

        void ISpecificationRunListener.OnContextStart(string contextInfoXml)
        {
            this.OnContextStart(ContextInfo.Parse(contextInfoXml));
        }

        void ISpecificationRunListener.OnContextEnd(string contextInfoXml)
        {
            this.OnContextEnd(ContextInfo.Parse(contextInfoXml));
        }

        void ISpecificationRunListener.OnSpecificationStart(string specificationInfoXml)
        {
            this.OnSpecificationStart(SpecificationInfo.Parse(specificationInfoXml));
        }

        void ISpecificationRunListener.OnSpecificationEnd(string specificationInfoXml, string resultXml)
        {
            this.OnSpecificationEnd(SpecificationInfo.Parse(specificationInfoXml), Result.Parse(resultXml));
        }

        void ISpecificationRunListener.OnFatalError(string exceptionResultXml)
        {
            this.OnFatalError(ExceptionResult.Parse(exceptionResultXml));
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
                    listener.OnAssemblyStart(element.XPathSelectElement("//onassemblystart/*").ToString());
                    break;
                case "onassemblyend":
                    listener.OnAssemblyEnd(element.XPathSelectElement("//onassemblyend/*").ToString());
                    break;
                case "onrunstart":
                    listener.OnRunStart();
                    break;
                case "onrunend":
                    listener.OnRunEnd();
                    break;
                case "oncontextstart":
                    listener.OnContextStart(element.XPathSelectElement("//oncontextstart/*").ToString());
                    break;
                case "oncontextend":
                    listener.OnContextEnd(element.XPathSelectElement("//oncontextend/*").ToString());
                    break;
                case "onspecificationstart":
                    listener.OnSpecificationStart(element.XPathSelectElement("//onspecificationstart/*").ToString());
                    break;
                case "onspecificationend":
                    listener.OnSpecificationEnd(
                        element.XPathSelectElement("//onspecificationend/specificationinfo").ToString(), 
                        element.XPathSelectElement("//onspecificationend/result").ToString());
                    break;
                case "onfatalerror":
                    listener.OnFatalError(element.XPathSelectElement("//onfatalerror/*").ToString());
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
