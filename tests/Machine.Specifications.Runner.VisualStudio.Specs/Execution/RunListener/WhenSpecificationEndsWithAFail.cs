using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(ProxyAssemblySpecificationRunListener))]
    class WhenSpecificationEndsWithAFail : WithFakes
    {
        static ProxyAssemblySpecificationRunListener run_listener;

        protected static TestCase test_case;

        static SpecificationInfo specification_info = new SpecificationInfo("leader", "field name", "ContainingType", "field_name");

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordEnd(Param<TestCase>.IsAnything, Param<TestOutcome>.IsAnything))
                .Callback((TestCase testCase, TestOutcome outcome) => test_case = testCase);

            run_listener = new ProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"));
        };


        Because of = () =>
            run_listener.OnSpecificationEnd(specification_info, Result.Failure(new NotImplementedException()));

        It should_notify_visual_studio_of_the_test_outcome = () =>
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordEnd(Param<TestCase>.IsNotNull, Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Failed)))
                .OnlyOnce();

        It should_notify_visual_studio_of_the_test_result = () =>
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordResult(Param<TestResult>.Matches(result =>
                    result.Outcome == TestOutcome.Failed &&
                    result.ComputerName == Environment.MachineName &&
                    result.ErrorMessage == new NotImplementedException().Message &&
                    !string.IsNullOrWhiteSpace(result.ErrorStackTrace)
                )))
                .OnlyOnce();

        It should_provide_correct_details_to_visual_studio = () =>
        {
            test_case.FullyQualifiedName.ShouldEqual("ContainingType::field_name");
            test_case.ExecutorUri.ShouldEqual(new Uri("bla://executorUri"));
            test_case.Source.ShouldEqual("assemblyPath");
        };
    }
}
