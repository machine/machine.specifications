using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(ProxyAssemblySpecificationRunListener))]
    class WhenSpecificationStarts : WithFakes
    {
        static ProxyAssemblySpecificationRunListener run_listener;

        protected static TestCase test_case;

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordStart(Param<TestCase>.IsAnything))
                .Callback((TestCase testCase) => test_case = testCase);

            run_listener = new ProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"));
        };

        Because of = () =>
            run_listener.OnSpecificationStart(new SpecificationInfo("leader", "field name", "ContainingType", "field_name"));

        It should_notify_visual_studio = () =>
            The<IFrameworkHandle>().WasToldTo(f => f.RecordStart(Param<TestCase>.IsNotNull));

        It should_provide_correct_details_to_visual_studio = () =>
        {
            test_case.FullyQualifiedName.ShouldEqual("ContainingType::field_name");
            test_case.ExecutorUri.ShouldEqual(new Uri("bla://executorUri"));
            test_case.Source.ShouldEqual("assemblyPath");
        };
    }
}
