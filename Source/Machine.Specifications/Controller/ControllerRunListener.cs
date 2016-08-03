using System;
using System.Xml.Linq;
using Machine.Specifications.Runner;


namespace Machine.Specifications.Controller
{

    internal class ControllerRunListener : ISpecificationRunListener
    {
        private readonly Action<string> _listener;

        public ControllerRunListener(Action<string> listener)
        {
            _listener = listener;
        }

        private void SendMessage(XElement message)
        {
            message.Add(new XAttribute("version", Controller.Version));
            _listener(message.ToString());
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblystart", assemblyInfo.ToXml()));

            SendMessage(root);
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            var root = new XElement("listener", new XElement("onassemblyend", assemblyInfo.ToXml()));

            SendMessage(root);
        }

        public void OnRunStart()
        {
            var root = new XElement("listener", new XElement("onrunstart"));

            SendMessage(root);
        }

        public void OnRunEnd()
        {
            var root = new XElement("listener", new XElement("onrunend"));

            SendMessage(root);
        }

        public void OnContextStart(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextstart", context.ToXml()));

            SendMessage(root);
        }

        public void OnContextEnd(ContextInfo context)
        {
            var root = new XElement("listener", new XElement("oncontextend", context.ToXml()));

            SendMessage(root);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var root = new XElement("listener", new XElement("onspecificationstart", specification.ToXml()));

            SendMessage(root);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var root = new XElement("listener", new XElement("onspecificationend", specification.ToXml(), result.ToXml()));

            SendMessage(root);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            var root = new XElement("listener", new XElement("onfatalerror", exception.ToXml()));

            SendMessage(root);
        }
    }
}
