using Example.CustomDelegates;

using FluentAssertions;

namespace Machine.Specifications.Specs.Runner
{
  public class when_running_specs_with_custom_delegates : running_specs
  {
    Because of =
      () => runner.RunAssembly(typeof(Account).Assembly);

    It should_run_them_all =
      () => listener.SpecCount.Should().Be(3);
  }
}