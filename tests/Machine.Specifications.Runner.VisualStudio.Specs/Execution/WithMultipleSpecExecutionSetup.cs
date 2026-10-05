using System;
using System.Reflection;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class WithMultipleSpecExecutionSetup : WithFakes
    {
        static ISpecificationExecutor executor;

        static Assembly assembly;

        protected static VisualStudioTestIdentifier[] specifications_to_run;

        Establish context = () => 
        {
            executor = new SpecificationExecutor();

            assembly = typeof(StandardSpec).Assembly;
        };

        Because of = () => 
            executor.RunAssemblySpecifications(assembly.Location, specifications_to_run, new Uri("bla://executor"), The<IFrameworkHandle>());
    }
}
