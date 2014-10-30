using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            var root = new XElement("runner", new XElement("runmember", assembly.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root, member));
        }

        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            var root = new XElement("runner", new XElement("runassemblies", assemblies.ToXml()));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            var root = new XElement("runner", new XElement("runnamespace", assembly.ToXml(), new XElement("namespace", targetNamespace)));

            _remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
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
            return new XElement("assembly", Path.GetFullPath(path));
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
        public RemoteRunnerMessage(XElement root, MemberInfo info = null)
        {
            this.Properties = new Dictionary<string, object>
            {
                { "data", root.ToString() },
                { "member", info },
            };
        }

        public IDictionary Properties { get; private set; }
    }
}