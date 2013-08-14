using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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
    Because of =
      () => program.Run(new [] { GetPath("Example.dll") });

    It should_write_the_assembly_name =
      () => console.Lines.ShouldContain(l => l.Contains("Example"));

    It should_write_the_specifications =
      () => console.Lines.ShouldContain(
        "» should debit the from account by the amount transferred",
        "» should credit the to account by the amount transferred",
        "» should not allow the transfer");

    It should_write_the_contexts =
      () => console.Lines.ShouldContain(
        "Account Funds transfer, when transferring between two accounts",
        "Account Funds transfer, when transferring an amount larger than the balance of the from account"
        );

    It should_write_the_count_of_contexts =
      () => console.Lines.ShouldContain(l => l.Contains("Contexts: 3"));

    It should_write_the_count_of_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("Specifications: 6"));

    It should_write_the_run_time =
      () => console.Lines.ShouldContain(l => l.Contains("Time: "));
  }

  [Subject("Console runner")]
  public class when_running_a_specification_assembly_and_silent_is_set
    : ConsoleRunnerSpecs
  {
    Because of =
      () => program.Run(new [] { GetPath("Example.dll"), "--silent" });

    It should_not_write_the_assembly_name =
      () => console.Lines.ShouldNotContain(l => l.Contains("Example"));

    It should_not_write_the_contexts =
      () => console.Lines.ShouldEachConformTo(l => !l.StartsWith("Account Funds transfer"));

    It should_not_write_the_specifications =
      () => console.Lines.ShouldEachConformTo(l => !l.StartsWith("» should"));

    It should_write_the_count_of_contexts =
      () => console.Lines.ShouldContain(l => l.Contains("Contexts: 3"));

    It should_write_the_count_of_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("Specifications: 6"));

    It should_write_the_run_time =
      () => console.Lines.ShouldContain(l => l.Contains("Time: "));
  }

  [Subject("Console runner")]
  public class when_running_a_specification_assembly_and_progress_is_set
    : ConsoleRunnerSpecs
  {
    Because of =
      () => program.Run(new [] { GetPath("Example.dll"), "--progress" });

    It should_write_the_assembly_name =
      () => console.Lines.ShouldContain(l => l.Contains("Example"));

    It should_not_write_the_contexts =
      () => console.Lines.ShouldEachConformTo(l => !l.StartsWith("Account Funds transfer"));

    It should_not_write_the_specifications =
      () => console.Lines.ShouldEachConformTo(l => !l.StartsWith("» should"));

    It should_write_the_specification_results =
      () => console.Lines.ShouldContain("...***");

    It should_write_the_count_of_contexts =
      () => console.Lines.ShouldContain(l => l.Contains("Contexts: 3"));

    It should_write_the_count_of_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("Specifications: 6"));

    It should_write_the_run_time =
      () => console.Lines.ShouldContain(l => l.Contains("Time: "));
  }

  [Subject("Console runner")]
  public class when_specifying_a_missing_assembly_on_the_command_line
    : ConsoleRunnerSpecs
  {
    const string missingAssemblyName = "Some.Missing.Assembly.dll";
    public static ExitCode exitCode;

    Because of = ()=>
      exitCode = program.Run(new[] { missingAssemblyName });

    It should_output_an_error_message_with_the_name_of_the_missing_assembly = ()=>
      console.Lines.ShouldContain(string.Format(Resources.MissingAssemblyError, missingAssemblyName));

    It should_return_the_Error_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Error);
  }

  [Subject("Console runner")]
  public class when_a_specification_fails : ConsoleRunnerSpecs
  {
    public static ExitCode exitCode;

    Because of =
      () => exitCode = program.Run(new[] { GetPath("Example.Failing.dll") });

    It should_write_the_failure =
      () => console.Lines.ShouldContain(l => l.Contains("Exception"));

    It should_write_the_count_of_failed_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("1 failed"));

    It should_return_the_Failure_exit_code =
      () => exitCode.ShouldEqual(ExitCode.Failure);

    It should_separate_failures_from_the_rest_of_the_test_run =
      () => console.Output.ShouldMatch(String.Format("\\S{0}{0}{0}Failures:{0}{0}\\S", Regex.Escape(Environment.NewLine)));
  }

  [Subject("Console runner")]
  public class when_a_specification_fails_and_silent_is_set : ConsoleRunnerSpecs
  {
    public static ExitCode exitCode;

    Because of =
      () => exitCode = program.Run(new[] { GetPath("Example.Failing.dll"), "--silent", "--exclude", "example" });

    It should_write_the_count_of_failed_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("1 failed"));

    It should_return_the_failure_exit_code =
      () => exitCode.ShouldEqual(ExitCode.Failure);

    It should_write_a_summary_of_failing_specifications =
      () => console.Lines.ShouldContain("Failures:", "Scott Bellware, at any given moment", "» will fail (FAIL)");

    It should_write_failure_stack_traces =
      () => console.Lines.ShouldContain(l => l.Contains("hi scott, love you, miss you."));

    It should_separate_failures_from_the_rest_of_the_test_run =
      () => console.Output.ShouldMatch(String.Format("\\S{0}{0}Failures:{0}{0}\\S", Regex.Escape(Environment.NewLine)));
  }

  [Subject("Console runner")]
  public class when_a_specification_fails_and_progress_is_set : ConsoleRunnerSpecs
  {
    public static ExitCode exitCode;

    Because of =
      () => exitCode = program.Run(new[] { GetPath("Example.Failing.dll"), "--progress", "--exclude", "example" });

    It should_write_failed_specification_results =
      () => console.Lines.ShouldContain("F");

    It should_write_the_count_of_failed_specifications =
      () => console.Lines.ShouldContain(l => l.Contains("1 failed"));

    It should_return_the_failure_exit_code =
      () => exitCode.ShouldEqual(ExitCode.Failure);

    It should_write_a_summary_of_failing_specifications =
      () => console.Lines.ShouldContain("Failures:", "Scott Bellware, at any given moment", "» will fail (FAIL)");

    It should_write_failure_stack_traces =
      () => console.Lines.ShouldContain(l => l.Contains("hi scott, love you, miss you."));

    It should_separate_failures_from_the_rest_of_the_test_run =
      () => console.Output.ShouldMatch(String.Format("\\S{0}{0}{0}Failures:{0}{0}\\S", Regex.Escape(Environment.NewLine)));
  }

  [Subject("Console runner")]
  public class when_specifying_an_include_filter : ConsoleRunnerSpecs
  {
    Because of = ()=>
      program.Run(new [] { GetPath("Example.dll"), "--include", "failure"});

    It should_execute_specs_with_the_included_tag = () =>
      console.Lines.ShouldContain(l => l.Contains("Account Funds transfer, when transferring an amount larger than the balance of the from account"));

    It should_not_execute_specs_that_are_not_included = () =>
      console.Lines.ShouldNotContain("Account Funds transfer, when transferring between two accounts");
  }

  [Subject("Console runner")]
  public class when_running_from_directory_different_from_assembly_location : ConsoleRunnerSpecs
  {
    Because of = () =>
      program.Run(new[] { GetPath(@"ExternalFile\Example.UsingExternalFile.dll") });

    It should_pass_the_specification_which_depends_on_external_file = () =>
      console.Lines.ShouldContain(
        "External resources usage, when using file copied to assembly output directory",
        "» should be able to locate it by relative path");

    It should_pass_all_specifications = () =>
      console.Lines.ShouldNotContain("failed");
  }
  
  [Subject("Console runner")]
  [Tags("Issue-157")]
  public class when_running_two_spec_assemblies_and_the_first_has_failing_specifications : ConsoleRunnerSpecs
  {
    static ExitCode ExitCode;

    Because of = () =>
      ExitCode = program.Run(new[] { GetPath(@"Issue157\Example.Issue157-Fail.dll"), GetPath(@"Issue157\Example.Issue157-Success.dll") });

    It should_fail_the_run = () =>
      ExitCode.ShouldNotEqual(ExitCode.Success);
  }
  
  [Subject("Console runner")]
  [Tags("Issue-157")]
  public class when_running_two_spec_assemblies_and_the_second_has_failing_specifications : ConsoleRunnerSpecs
  {
    static ExitCode ExitCode;

    Because of = () =>
      ExitCode = program.Run(new[] { GetPath(@"Issue157\Example.Issue157-Success.dll"), GetPath(@"Issue157\Example.Issue157-Fail.dll") });

    It should_fail_the_run = () =>
      ExitCode.ShouldNotEqual(ExitCode.Success);
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