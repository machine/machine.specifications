using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class AssemblyLocationAwareRunListener : ISpecificationRunListener
    {
        private readonly IEnumerable<Assembly> assemblies;

        public AssemblyLocationAwareRunListener(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            var loadedAssembly = assemblies.FirstOrDefault(a => a.GetName().Name.Equals(assembly.Name, StringComparison.OrdinalIgnoreCase));

            Directory.SetCurrentDirectory(Path.GetDirectoryName(loadedAssembly.Location));
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }
    }
}
