using System;
using System.Linq;
using System.Text;

using Machine.Specifications.ConsoleRunner.Properties;

using Rhino.Mocks;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  [Concerning(typeof(Runner))]
  public class When_running_with_no_command_line_arguments
    : with_runner 
  {
    Because of = ()=>
      runner.Run(new string[] {});

    It should_print_usage_statement = ()=>
      console.Lines.ShouldContain(Resources.UsageStatement);
  }

  [Concerning(typeof(Runner))]
  public class When_running_with_one_assembly_on_the_command_line
    : with_runner 
  {
    Because of = ()=>
      runner.Run(new [] {"Machine.Specifications.Example.dll"});

    It should_output_the_results_from_all_the_specifications_in_that_assembly = ()=>
      console.Lines.ShouldContain(
        "should debit the from account by the amount transferred", 
        "should credit the to account by the amount transferred", 
        "should not allow the transfer");
  }

  [Concerning(typeof(Runner))]
  public class When_specifying_a_missing_assembly_on_the_command_line
    : with_runner 
  {
    const string missingAssemblyName = "Some.Missing.Assembly.dll";
    public static ExitCode exitCode;

    Because of = ()=>
      exitCode = runner.Run(new string[] {missingAssemblyName});

    It should_output_an_error_message_with_the_name_of_the_missing_assembly = ()=>
      console.Lines.ShouldContain(string.Format(Resources.MissingAssemblyError, missingAssemblyName));

    It should_return_the_Error_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Error);
  }

  [Concerning(typeof(Runner))]
  public class When_a_specification_fails : with_runner
  {
    public static ExitCode exitCode;
    const string assemblyWithFailingSpecification = "Machine.Specifications.FailingExample";
    const string failingSpecificationName = "should fail";

    Because of = ()=>
      exitCode = runner.Run(new string[] {assemblyWithFailingSpecification + ".dll"});

    It should_output_the_name_of_the_failing_specification = ()=>
      console.Lines.ShouldContain(string.Format(Resources.FailingSpecificationError, failingSpecificationName, assemblyWithFailingSpecification));

    It should_return_the_Failure_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Failure);
  }

  public class with_runner
  {
    public static Runner runner;
    public static FakeConsole console;

    Establish context = ()=>
    {
      console = new FakeConsole();
      runner = new Runner(console);
    };
  }
}