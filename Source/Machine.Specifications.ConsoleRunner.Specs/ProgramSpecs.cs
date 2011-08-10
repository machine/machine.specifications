using System;
using System.IO;
using System.Reflection;
using Machine.Specifications.ConsoleRunner.Properties;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  // TODO: Add Subject count
  // TODO: Add Tag and filter by tag
  // TODO: Add awesome client side reporting stuff

  [Subject("Console runner")]
  public class when_arguments_are_not_provided
    : ConsoleRunnerSpecs 
  {
    Because of = ()=>
      program.Run(new string[] {});

    It should_print_usage_statement = ()=>
      console.Lines.ShouldContain(Options.Usage());
  }

  [Subject("Console runner")]
  public class when_running_a_specification_assembly
    : ConsoleRunnerSpecs 
  {
    Because of = ()=>
      program.Run(new [] {GetPath("Machine.Specifications.Example.dll")});

    It should_write_the_assembly_name = ()=>
      console.ShouldContainLineWith("Machine.Specifications.Example");

    It should_write_the_specifications = ()=>
      console.Lines.ShouldContain(
        "» should debit the from account by the amount transferred", 
        "» should credit the to account by the amount transferred", 
        "» should not allow the transfer");

    It should_write_the_contexts = ()=>
      console.Lines.ShouldContain(
        "Account Funds transfer, when transferring between two accounts",
        "Account Funds transfer, when transferring an amount larger than the balance of the from account"
        );

    It should_write_the_count_of_contexts = ()=>
      console.ShouldContainLineWith("Contexts: 3");

    It should_write_the_count_of_specifications = ()=>
      console.ShouldContainLineWith("Specifications: 6");
    
    It should_write_the_run_time = ()=>
      console.ShouldContainLineWith("Time: ");
  }

  [Subject("Console runner")]
  public class when_specifying_a_missing_assembly_on_the_command_line
    : ConsoleRunnerSpecs 
  {
    const string missingAssemblyName = "Some.Missing.Assembly.dll";
    public static ExitCode exitCode;

    Because of = ()=>
      exitCode = program.Run(new string[] {missingAssemblyName});

    It should_output_an_error_message_with_the_name_of_the_missing_assembly = ()=>
      console.Lines.ShouldContain(string.Format(Resources.MissingAssemblyError, missingAssemblyName));

    It should_return_the_Error_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Error);
  }

  [Subject("Console runner")]
  public class when_a_specification_fails : ConsoleRunnerSpecs
  {
    public static ExitCode exitCode;
    const string assemblyWithFailingSpecification = "Machine.Specifications.FailingExample";
    const string failingSpecificationName = "should fail";

    Because of = ()=>
      exitCode = program.Run(new string[] {GetPath(assemblyWithFailingSpecification + ".dll")});

    It should_write_the_failure = ()=>
      console.ShouldContainLineWith("Exception");

    It should_write_the_count_of_failed_specifications = ()=>
      console.ShouldContainLineWith("1 failed");

    It should_return_the_Failure_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Failure);
  }

  [Subject("Console runner")]
  public class when_a_specification_fails_and_silent_is_set : ConsoleRunnerSpecs
  {
    public static ExitCode exitCode;
    const string assemblyWithFailingSpecification = "Machine.Specifications.FailingExample";
    const string failingSpecificationName = "should fail";

    Because of = ()=>
      exitCode = program.Run(new string[] { GetPath(assemblyWithFailingSpecification + ".dll"), "-s"});

    It should_write_the_count_of_failed_specifications = ()=>
      console.ShouldContainLineWith("1 failed");

    It should_return_the_Failure_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Failure);
  }

  [Subject("Console runner")]
  public class when_specifying_an_include_filter : ConsoleRunnerSpecs
  {
    Because of = ()=>
      program.Run(new [] { GetPath("Machine.Specifications.Example.dll"), "--include", "failure"});

    It should_execute_specs_with_the_included_tag = () =>
      console.ShouldContainLineWith(
        "Account Funds transfer, when transferring an amount larger than the balance of the from account");

    It should_not_execute_specs_that_are_not_included = () =>
      console.ShouldNotContainLineWith("Account Funds transfer, when transferring between two accounts");
  }

  [Subject("Console runner")]
  public class when_running_from_directory_different_from_assembly_location : ConsoleRunnerSpecs
  {
    Because of = () =>
      program.Run(new[] { GetPath(@"ExternalFile\Machine.Specifications.Example.UsingExternalFile.dll") });

    It should_pass_the_specification_which_depends_on_external_file = () =>
      console.Lines.ShouldContain(
        "External resources usage, when using file copied to assembly output directory", 
        "» should be able to locate it by relative path");

    It should_pass_all_specifications = () =>
      console.ShouldNotContainLineWith("failed");
  }

  public class ConsoleRunnerSpecs
  {
    const string TeamCityIndicator = "TEAMCITY_PROJECT_NAME";
    static string TeamCityEnvironment;

    public static Program program;
    public static FakeConsole console;

    Establish context = () =>
    {
      console = new FakeConsole();
      program = new Program(console);

      TeamCityEnvironment = Environment.GetEnvironmentVariable(TeamCityIndicator);
      Environment.SetEnvironmentVariable(TeamCityIndicator, String.Empty);
    };

    Cleanup after = () =>
    {
      if (TeamCityEnvironment != null)
      {
        Environment.SetEnvironmentVariable(TeamCityIndicator, TeamCityEnvironment);
      }
    };

    protected static string GetPath(string path)
    {
      return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
    }
  }
}