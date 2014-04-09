using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Sdk
{
    public class RunSpecs : RemoteToInternalSpecificationRunListenerAdapter
    {
        public RunSpecs(object listener, string runOptionsXml, IEnumerable<Assembly> testAssemblies)
            : base(listener)
        {
            var runOptions = RunOptions.Parse(runOptionsXml);
            var runner = new AppDomainRunner(this, runOptions);

            if (runOptions.Contexts.Any())
            {
                Assembly testAssembly = testAssemblies.Single();
                runner.StartRun(testAssembly);

                foreach (var contextClass in runOptions.Contexts.Select(testAssembly.GetType))
                {
                    runner.RunMember(testAssembly, contextClass);
                }

                runner.EndRun(testAssembly);
            }
            else
            {
                runner.RunAssemblies(testAssemblies);
            }
        }
    }
}