using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    internal class RemoteRunnerDecorator : ISpecificationRunner
    {
        private readonly IMessageSink _remoteRunner;

        public RemoteRunnerDecorator(IMessageSink remoteRunner)
        {
            _remoteRunner = remoteRunner;
        }

        public void StartRun(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("startrun", assembly.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void EndRun(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("endrun", assembly.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
            // TODO
        }

        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            var root = new XElement("runner", new XElement("runassemblies", assemblies.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            // TODO
        }

        public void RunAssembly(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("runassembly", assembly.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }
    }

    internal static class AssemblyPathExtensions
    {
        public static XElement ToXml(this AssemblyPath path)
        {
            return new XElement("assembly", path);
        }
    }

    internal static class AssemblyPathsExtensions
    {
        public static XElement ToXml(this IEnumerable<AssemblyPath> paths)
        {
            return new XElement("assemblies", paths.Select(p => p.ToXml()).ToList());
        }
    }

    internal class RemoteRunnerMessage : MarshalByRefObject, IMessage
    {
        public RemoteRunnerMessage(XElement root)
        {
            this.Properties = new Dictionary<string, string>
            {
                {
                    "data", root.ToString()
                }
            };
        }

        public IDictionary Properties { get; private set; }
    }
}