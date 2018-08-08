
using System.Linq;
using System.Reflection;

using Example;

using FluentAssertions;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Specs.Runner
{
    [Subject("Specification Runner")]
    public class when_running_a_context_while_filtering_out_failing_tests
    {
        static TestListener testListener;
    
        static DefaultRunner runner;

        Establish context = () =>
        {
            testListener = new TestListener();
            var filters = new[]
                          {
                              new ContextFilter(
                                  "Example.when_context_with_passing_and_failing_clauses",
                                  new[]
                                  {
                                      new SpecifiactionFilter("should_pass"),
                                  })
                          };
            var runOptions = new RunOptions(
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                filters);

            runner = new DefaultRunner(
                testListener,
                runOptions);
        };

        Because of =
            () => runner.RunAssemblyContainingType<when_context_with_passing_and_failing_clauses>();
        
        It should_succeed =
            () => testListener.LastResult.Passed.Should().BeTrue();
    
        It should_run_spec =
            () => testListener.SpecCount.Should().Be(1);
    }

    public static class RunnerExtensions
    {
        public static void RunAssemblyContainingType<T>(
            this DefaultRunner runner)
            where T : new()
        {
            runner.RunAssembly(typeof(T).GetTypeInfo().Assembly);
        }
    }
}