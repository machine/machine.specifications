#if !NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    internal class RemoteRunnerDecorator : ISpecificationRunner
    {
        private readonly IMessageSink remoteRunner;

        public RemoteRunnerDecorator(IMessageSink remoteRunner)
        {
            this.remoteRunner = remoteRunner;
        }

        public void StartRun(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("startrun", assembly.ToXml()));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void EndRun(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("endrun", assembly.ToXml()));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunMember(AssemblyPath assembly, MemberInfo member)
        {
            var root = new XElement("runner", new XElement("runmember", assembly.ToXml()));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root, member));
        }

        public void RunAssemblies(IEnumerable<AssemblyPath> assemblies)
        {
            var root = new XElement("runner", new XElement("runassemblies", assemblies.ToXml()));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunNamespace(AssemblyPath assembly, string targetNamespace)
        {
            var root = new XElement("runner", new XElement("runnamespace", assembly.ToXml(), new XElement("namespace", targetNamespace)));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }

        public void RunAssembly(AssemblyPath assembly)
        {
            var root = new XElement("runner", new XElement("runassembly", assembly.ToXml()));

            remoteRunner.SyncProcessMessage(new RemoteRunnerMessage(root));
        }
    }
}
#endif
