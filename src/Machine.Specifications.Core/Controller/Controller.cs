using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Machine.Specifications.Explorers;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Controller
{
    public class Controller
    {
        public static string Version => "1.0";

        private readonly DefaultRunner runner;

        private readonly AssemblyExplorer explorer;

        private readonly ISpecificationRunListener listener;

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
            listener = new ControllerRunListener(listenCallback);
            explorer = new AssemblyExplorer();
            runner = new DefaultRunner(listener, runOptions, false);
        }

        public void StartRun()
        {
            listener.OnRunStart();
        }

        public void EndRun()
        {
            listener.OnRunEnd();
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
            runner.RunAssemblies(assemblies);
        }

        public void RunNamespaces(Assembly assembly, IEnumerable<string> targetNamespaces)
        {
            try
            {
                runner.StartRun(assembly);

                foreach (var targetNamespace in targetNamespaces)
                {
                    runner.RunNamespace(assembly, targetNamespace);
                }
            }
            finally
            {
                runner.EndRun(assembly);
            }
        }

        public void RunMembers(Assembly assembly, IEnumerable<MemberInfo> members)
        {
            try
            {
                runner.StartRun(assembly);

                foreach (var member in members)
                {
                    runner.RunMember(assembly, member);
                }
            }
            finally
            {
                runner.EndRun(assembly);
            }
        }

        /// <summary>
        /// Run specifics specs. This method is available to support IDE integration scenarios, where users can run
        /// a specific test only.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="specifications">A spec full name "namespace.type.field_name". </param>
        /// <remarks>This method supports fields that actually come from Behaviors </remarks>
        public void RunSpecs(Assembly assembly, IEnumerable<string> specifications)
        {
            try
            {
                runner.StartRun(assembly);

                foreach (var specsByType in specifications.GroupBy(spec => spec.Substring(0, spec.LastIndexOf("."))))
                {
                    var fullTypeName = specsByType.Key;
                    var specNames = specsByType.Select(spec => spec.Substring(spec.LastIndexOf(".") + 1, spec.Length - 1 - spec.LastIndexOf(".")));

                    runner.RunType(assembly, assembly.GetType(fullTypeName), specNames);
                }
            }
            finally
            {
                runner.EndRun(assembly);
            }
        }

        public void RunTypes(Assembly assembly, IEnumerable<Type> types)
        {
            try
            {
                runner.StartRun(assembly);

                foreach (var type in types)
                {
                    runner.RunMember(assembly, type.GetTypeInfo());
                }
            }
            finally
            {
                runner.EndRun(assembly);
            }
        }

        public string DiscoverSpecs(Assembly assembly)
        {
            var contextListElement = new XElement("contexts");
            contextListElement.Add(new XAttribute("version", Version));

            var contexts = explorer.FindContextsIn(assembly);

            foreach (var context in contexts)
            {
                var contextInfoElement = context.GetInfo().ToXml();

                var specificationsListElement = new XElement("specifications");

                foreach (var spec in context.Specifications)
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
