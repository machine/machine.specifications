using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl
{
    [Serializable]
    public class DefaultRunner : MarshalByRefObject, ISpecificationRunner, IMessageSink
    {
        readonly ISpecificationRunListener _listener;
        readonly RunOptions _options;
        readonly AssemblyExplorer _explorer;
        readonly AssemblyRunner _assemblyRunner;
        InvokeOnce _runStart = new InvokeOnce(() => { });
        InvokeOnce _runEnd = new InvokeOnce(() => { });
        bool _explicitStartAndEnd;

        public DefaultRunner(object listener, string runOptionsXml, bool signalRunStartAndEnd)
            : this(new RemoteRunListenerDecorator(listener), RunOptions.Parse(runOptionsXml), signalRunStartAndEnd)
        {
        }

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
            var contexts = _explorer.FindContextsIn(assembly);
            var map = CreateMap(assembly, contexts);

            StartRun(map);
        }

        public void RunAssemblies(IEnumerable<Assembly> assemblies)
        {
            var map = new Dictionary<Assembly, IEnumerable<Context>>();

            assemblies.Each(assembly => map.Add(assembly, _explorer.FindContextsIn(assembly)));

            StartRun(map);
        }

        public void RunNamespace(Assembly assembly, string targetNamespace)
        {
            var contexts = _explorer.FindContextsIn(assembly, targetNamespace);

            StartRun(CreateMap(assembly, contexts));
        }

        public void RunMember(Assembly assembly, MemberInfo member)
        {
            if (member.MemberType == MemberTypes.TypeInfo ||
                member.MemberType == MemberTypes.NestedType)
            {
                RunClass(member, assembly);
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                RunField(member, assembly);
            }
        }

        void RunField(MemberInfo member, Assembly assembly)
        {
            var fieldInfo = (FieldInfo)member;
            var context = _explorer.FindContexts(fieldInfo);

            StartRun(CreateMap(assembly, new[] { context }));
        }

        void RunClass(MemberInfo member, Assembly assembly)
        {
            var type = (Type)member;
            var context = _explorer.FindContexts(type);

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

            foreach (var pair in contextMap)
            {
                var assembly = pair.Key;
                // TODO: move this filtering to a more sensible place
                var contexts = pair.Value.FilteredBy(_options);

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
    }

    public static class TagFilteringExtensions
    {
        public static IEnumerable<Context> FilteredBy(this IEnumerable<Context> contexts, RunOptions options)
        {
            var results = contexts;

            if (options.Filters.Any())
            {
                var includeFilters = options.Filters;

                results = results.Where(x => includeFilters.Any(filter => StringComparer.OrdinalIgnoreCase.Equals(filter, x.Type.FullName)));
            }

            if (options.Specifications.Any())
            {
                var includeSpecs = options.Specifications;

                Func<Context, Specification,string> format = (context, spec) => {
                    return String.Format("{0}::{1}::{2}", context.Type.Assembly.GetName().Name, context.Type.FullName, spec.FieldInfo.Name);
                };

                foreach (Context context in contexts) {
                    context.Specifications
                        .Where(spec => !includeSpecs.Any(includedSpec => includedSpec.Equals(format(context, spec), StringComparison.OrdinalIgnoreCase)))
                        .ToList()
                        .ForEach(spec => spec.IsIgnored = true);
                }
            }

            if (options.IncludeTags.Any())
            {
                var tags = options.IncludeTags.Select(tag => new Tag(tag));

                results = results.Where(x => x.Tags.Intersect(tags).Any());
            }

            if (options.ExcludeTags.Any())
            {
                var tags = options.ExcludeTags.Select(tag => new Tag(tag));
                results = results.Where(x => !x.Tags.Intersect(tags).Any());
            }

            return results;
        }
    }
}