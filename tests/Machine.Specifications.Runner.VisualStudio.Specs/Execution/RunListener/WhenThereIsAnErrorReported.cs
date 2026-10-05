using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(ProxyAssemblySpecificationRunListener))]
    class WhenThereIsAnErrorReported : WithFakes
    {
        static ProxyAssemblySpecificationRunListener run_listener;

        Establish context = () => 
            run_listener = new ProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"));

        Because of = () => 
            run_listener.OnFatalError(new ExceptionResult(new InvalidOperationException()));

        It should_notify_visual_studio_of_the_error_outcome = () =>
            The<IFrameworkHandle>()
                .WasToldTo(f => f.SendMessage(
                    Param<TestMessageLevel>.Matches(level => level == TestMessageLevel.Error),
                    Param<string>.Matches(message => message.Contains("InvalidOperationException"))))
                .OnlyOnce();
    }
}
