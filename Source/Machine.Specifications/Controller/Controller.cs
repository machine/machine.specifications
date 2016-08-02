using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Controller
{
    public class Controller
    {
        internal static readonly string PROTOCOL_VERSION = "1.0";

        readonly DefaultRunner _runner;
        readonly AssemblyExplorer _explorer;
        readonly ISpecificationRunListener _listener;

        public Controller(Action<string> listenCallback)
            : this(listenCallback, RunOptions.Default)
        {
        }


        public Controller(Action<string> listenCallback, string runOptions)
            :this(listenCallback, RunOptions.Parse(runOptions))
        {
        }

        private Controller(Action<string> listenCallback, RunOptions runOptions)
        {
            _listener = new ControllerRunListener(listenCallback);
            _explorer = new AssemblyExplorer();
            _runner = new DefaultRunner(_listener, runOptions, signalRunStartAndEnd: false);
        }

        public void StartRun()
        {
            _listener.OnRunStart();
        }

        public void EndRun()
        {
            _listener.OnRunEnd();
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                _runner.RunAssemblies(assemblies);
            }
        }

        public void RunNamespaces(Assembly assembly, IEnumerable<string> targetNamespaces)
        {
            try
            {
                _runner.StartRun(assembly);
                foreach (string targetNamespace in targetNamespaces)
                {
                    _runner.RunNamespace(assembly, targetNamespace);
                }
            }
            finally
            {
                _runner.EndRun(assembly);
            }
        }

        public void RunMembers(Assembly assembly, IEnumerable<MemberInfo> members)
        {
            try
            {
                _runner.StartRun(assembly);

                foreach (MemberInfo member in members)
                {
                    _runner.RunMember(assembly, member);
                }
            }
            finally
            {
                _runner.EndRun(assembly);
            }
        }

        public void RunTypes(Assembly assembly, IEnumerable<Type> types)
        {
            try
            {
                _runner.StartRun(assembly);

                foreach (Type type in types)
                {
                    _runner.RunMember(assembly, type.GetTypeInfo());
                }
            }
            finally
            {
                _runner.EndRun(assembly);
            }
        }

        public string DiscoverSpecs(Assembly assembly)
        {
            XElement contextListElement = new XElement("contexts");
            contextListElement.Add(new XAttribute("version", "1.0.0"));

            IEnumerable<Context> contexts = _explorer.FindContextsIn(assembly);
            foreach (Context context in contexts)
            {
                XElement contextInfoElement = context.GetInfo().ToXml();

                XElement specificationsListElement = new XElement("specifications");
                foreach (Specification spec in context.Specifications)
                {
                	specificationsListElement.Add(spec.GetInfo().ToXml());
                }

                contextInfoElement.Add(specificationsListElement);
                contextListElement.Add(contextInfoElement);
            }

            return contextListElement.ToString();
        }
    }
}
