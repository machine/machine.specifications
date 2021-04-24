using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

#if !NETSTANDARD
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Security;
#endif

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
        readonly ISpecificationRunListener _listener;
        readonly RunOptions _options;
        readonly AssemblyExplorer _explorer;
        readonly AssemblyRunner _assemblyRunner;
        InvokeOnce _runStart = new InvokeOnce(() => { });
        InvokeOnce _runEnd = new InvokeOnce(() => { });
        bool _explicitStartAndEnd;

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
            _listener = listener;
            _options = options;
            _assemblyRunner = new AssemblyRunner(_listener, _options);

            _explorer = new AssemblyExplorer();

            if (signalRunStartAndEnd)
            {
                _runStart = new InvokeOnce(() => _listener.OnRunStart());
                _runEnd = new InvokeOnce(() => _listener.OnRunEnd());
            }
        }

        public void RunAssembly(Assembly assembly)
        {
            var contexts = _explorer.FindContextsIn(assembly, _options);
            var map = CreateMap(assembly, contexts);

            StartRun(map);
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
            var map = new Dictionary<Assembly, IEnumerable<Context>>();

            assemblies.Each(assembly => map.Add(assembly, _explorer.FindContextsIn(assembly, _options)));

            StartRun(map);
        }

        public void RunNamespace(Assembly assembly, string targetNamespace)
        {
            var contexts = _explorer.FindContextsIn(assembly, targetNamespace, _options);

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
            Context context = _explorer.FindContexts(type, _options);
            IEnumerable<Specification> specsToRun = context.Specifications.Where(s => specs.Contains(s.FieldInfo.Name));
            context.Filter(specsToRun);

            StartRun(CreateMap(assembly, new[] { context }));
        }

        void RunField(MemberInfo member, Assembly assembly)
        {
            var fieldInfo = (FieldInfo)member;
            var context = _explorer.FindContexts(fieldInfo, _options);

            StartRun(CreateMap(assembly, new[] { context }));
        }

        void RunClass(MemberInfo member, Assembly assembly)
        {
            var type = member.AsType();
            var context = _explorer.FindContexts(type, _options);

            if (context == null)
            {
                return;
            }

            StartRun(CreateMap(assembly, new[] { context }));
        }

        static Dictionary<Assembly, IEnumerable<Context>> CreateMap(Assembly assembly, IEnumerable<Context> contexts)
        {
            var map = new Dictionary<Assembly, IEnumerable<Context>>();
            map[assembly] = contexts;
            return map;
        }

        void StartRun(IDictionary<Assembly, IEnumerable<Context>> contextMap)
        {
            if (!_explicitStartAndEnd)
            {
                _runStart.Invoke();
            }

            foreach (var (assembly, contexts) in contextMap)
            {
                _assemblyRunner.Run(assembly, contexts);
            }

            if (!_explicitStartAndEnd)
            {
                _runEnd.Invoke();
            }
        }

        public void StartRun(Assembly assembly)
        {
            _explicitStartAndEnd = true;
            _runStart.Invoke();
            _assemblyRunner.StartExplicitRunScope(assembly);
        }

        public void EndRun(Assembly assembly)
        {
            _assemblyRunner.EndExplicitRunScope(assembly);
            _runEnd.Invoke();
        }

#if !NETSTANDARD
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
            var element = doc.XPathSelectElement("/runner/*");

            XElement assemblyElement;
            switch (element.Name.ToString())
            {
                // TODO: Optimize loading of assemblies
                case "startrun":
                    this.StartRun(Assembly.LoadFrom(element.Value));
                    break;
                case "endrun":
                    this.EndRun(Assembly.LoadFrom(element.Value));
                    break;
                case "runassembly":
                    this.RunAssembly(Assembly.LoadFrom(element.Value));
                    break;
                case "runnamespace":
                    assemblyElement = element.XPathSelectElement("/runner/runnamespace/assembly");
                    var namespaceElement = element.XPathSelectElement("/runner/runnamespace/namespace");

                    this.RunNamespace(Assembly.LoadFrom(assemblyElement.Value), namespaceElement.Value);
                    break;
                case "runmember":
                    assemblyElement = element.XPathSelectElement("/runner/runmember/assembly");
                    var memberInfo = msg.Properties["member"] as MemberInfo;

                    this.RunMember(Assembly.LoadFrom(assemblyElement.Value), memberInfo);
                    break;
                case "runassemblies":
                    this.RunAssemblies(element.Elements("assemblies").Select(e => Assembly.LoadFrom(e.Value)));
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
