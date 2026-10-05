using System;
using System.Reflection;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class WithSingleSpecExecutionSetup : WithFakes
    {
        static ISpecificationExecutor executor;

        static Assembly assembly;

        protected static VisualStudioTestIdentifier specification_to_run;

        Establish context = () =>
        {
            executor = new SpecificationExecutor();

            assembly = typeof(StandardSpec).Assembly;
        };

        Because of = () =>
            executor.RunAssemblySpecifications(assembly.Location, new[] { specification_to_run }, new Uri("bla://executor"), The<IFrameworkHandle>());
    }
}
