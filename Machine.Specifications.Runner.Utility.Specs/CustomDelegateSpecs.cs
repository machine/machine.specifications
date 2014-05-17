using Example.CustomDelegates;
using FluentAssertions;

namespace Machine.Specifications.Runner.Utility
{
    public class when_running_specs_with_custom_delegates : running_specs
    {
        Because of =
          () => runner.RunAssembly(new AssemblyPath(typeof(Account).Assembly.Location));

        It should_run_them_all =
          () => listener.SpecCount.Should().Be(3);
    }
}