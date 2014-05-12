using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Sdk
{
    public sealed class RunSpecs : RemoteToInternalSpecificationRunListenerAdapter
    {
        public RunSpecs(object listener, string runOptionsXml, IEnumerable<string> assemblyPaths)
            : base(listener, runOptionsXml)
        {
            var assemblies = assemblyPaths.Select(Assembly.LoadFile);

            // TODO: What to do with that?
            if (RunOptions.Contexts.Any())
            {
                var assembly = assemblies.Single();
                Runner.StartRun(assembly);

                foreach (var contextClass in RunOptions.Contexts.Select(assembly.GetType))
                {
                    Runner.RunMember(assembly, contextClass);
                }

                Runner.EndRun(assembly);
            }
            else
            {
                Runner.RunAssemblies(assemblies);
            }
        }
    }
}