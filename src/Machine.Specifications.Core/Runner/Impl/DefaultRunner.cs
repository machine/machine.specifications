using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NETSTANDARD
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class DefaultRunner :
#if !NETSTANDARD
                                MarshalByRefObject, IMessageSink,
#endif
                                ISpecificationRunner

    {
        private readonly ISpecificationRunListener listener;

        private readonly RunOptions options;

        private readonly AssemblyExplorer explorer;

        private readonly AssemblyRunner assemblyRunner;

        private InvokeOnce runStart = new InvokeOnce(() => { });

        private InvokeOnce runEnd = new InvokeOnce(() => { });

        private bool explicitStartAndEnd;

#if !NETSTANDARD
        public DefaultRunner(object listener, string runOptionsXml, bool signalRunStartAndEnd)
            : this(new RemoteRunListenerDecorator(listener), RunOptions.Parse(runOptionsXml), signalRunStartAndEnd)
        {
        }
#endif

        public DefaultRunner(ISpecificationRunListener listener, RunOptions options)
            : this(listener, options, true)
        {
        }

        public DefaultRunner(ISpecificationRunListener listener, RunOptions options, bool signalRunStartAndEnd)
        {
            this.listener = listener;
            this.options = options;

            assemblyRunner = new AssemblyRunner(this.listener, this.options);

            explorer = new AssemblyExplorer();

            if (signalRunStartAndEnd)
            {
                runStart = new InvokeOnce(() => this.listener.OnRunStart());
                runEnd = new InvokeOnce(() => this.listener.OnRunEnd());
            }
        }

        public void RunAssembly(Assembly assembly)
        {
            var contexts = explorer.FindContextsIn(assembly, options);
            var map = CreateMap(assembly, contexts);

            StartRun(map);
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
            var map = new Dictionary<Assembly, IEnumerable<Context>>();

            assemblies.Each(assembly => map.Add(assembly, explorer.FindContextsIn(assembly, options)));

            StartRun(map);
        }

        public void RunNamespace(Assembly assembly, string targetNamespace)
        {
            var contexts = explorer.FindContextsIn(assembly, targetNamespace, options);

            StartRun(CreateMap(assembly, contexts));
        }

        public void RunMember(Assembly assembly, MemberInfo member)
        {
            if (member.IsType())
            {
                RunClass(member, assembly);
            }
            else if (member is FieldInfo)
            {
                RunField(member, assembly);
            }
        }

        public void RunType(Assembly assembly, Type type, IEnumerable<string> specs)
        {
            var context = explorer.FindContexts(type, options);
            var specsToRun = context.Specifications.Where(s => specs.Contains(s.FieldInfo.Name));
            context.Filter(specsToRun);

            StartRun(CreateMap(assembly, new[] { context }));
        }

        private void RunField(MemberInfo member, Assembly assembly)
        {
            var fieldInfo = (FieldInfo)member;
            var context = explorer.FindContexts(fieldInfo, options);

            StartRun(CreateMap(assembly, new[] { context }));
        }

        private void RunClass(MemberInfo member, Assembly assembly)
        {
            var type = member.AsType();
            var context = explorer.FindContexts(type, options);

            if (context == null)
            {
                return;
            }

            StartRun(CreateMap(assembly, new[] { context }));
        }

        private static Dictionary<Assembly, IEnumerable<Context>> CreateMap(Assembly assembly, IEnumerable<Context> contexts)
        {
            return new Dictionary<Assembly, IEnumerable<Context>>
            {
                [assembly] = contexts
            };
        }

        private void StartRun(IDictionary<Assembly, IEnumerable<Context>> contextMap)
        {
            if (!explicitStartAndEnd)
            {
                runStart.Invoke();
            }

            foreach (var (assembly, contexts) in contextMap)
            {
                assemblyRunner.Run(assembly, contexts);
            }

            if (!explicitStartAndEnd)
            {
                runEnd.Invoke();
            }
        }

        public void StartRun(Assembly assembly)
        {
            explicitStartAndEnd = true;
            runStart.Invoke();
            assemblyRunner.StartExplicitRunScope(assembly);
        }

        public void EndRun(Assembly assembly)
        {
            assemblyRunner.EndExplicitRunScope(assembly);
            runEnd.Invoke();
        }

#if !NETSTANDARD
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            if (msg is IMethodCallMessage methodCall)
            {
                return RemotingServices.ExecuteMessage(this, methodCall);
            }

            // This is all a bit ugly but gives us version independance for the moment
            var value = msg.Properties["data"] as string;
            var doc = XDocument.Load(new StringReader(value));
            var element = doc.XPathSelectElement("/runner/*");

            XElement assemblyElement;

            switch (element.Name.ToString())
            {
                // TODO: Optimize loading of assemblies
                case "startrun":
                    StartRun(Assembly.LoadFrom(element.Value));
                    break;

                case "endrun":
                    EndRun(Assembly.LoadFrom(element.Value));
                    break;

                case "runassembly":
                    RunAssembly(Assembly.LoadFrom(element.Value));
                    break;

                case "runnamespace":
                    assemblyElement = element.XPathSelectElement("/runner/runnamespace/assembly");
                    var namespaceElement = element.XPathSelectElement("/runner/runnamespace/namespace");

                    RunNamespace(Assembly.LoadFrom(assemblyElement.Value), namespaceElement.Value);
                    break;

                case "runmember":
                    assemblyElement = element.XPathSelectElement("/runner/runmember/assembly");
                    var memberInfo = msg.Properties["member"] as MemberInfo;

                    RunMember(Assembly.LoadFrom(assemblyElement.Value), memberInfo);
                    break;

                case "runassemblies":
                    RunAssemblies(element.Elements("assemblies").Select(e => Assembly.LoadFrom(e.Value)));
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
