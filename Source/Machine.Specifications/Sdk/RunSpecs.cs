using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Sdk
{
    public sealed class RunSpecs : RemoteToInternalSpecificationRunListenerAdapter
    {
        public RunSpecs(object listener, string runOptionsXml, string assemblyPath)
            : base(listener, runOptionsXml)
        {
            var assembly = Assembly.LoadFile(assemblyPath);

            if (RunOptions.Contexts.Any())
            {
                Runner.StartRun(assembly);

                foreach (var contextClass in RunOptions.Contexts.Select(assembly.GetType))
                {
                    Runner.RunMember(assembly, contextClass);
                }

                Runner.EndRun(assembly);
            }
            else
            {
                Runner.RunAssembly(assembly);
            }
        }
    }
}