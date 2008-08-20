using System.Collections.Generic;
using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner.Specs
{
  // TODO: Add Concern count
  // TODO: Add Tag and filter by tag
  // TODO: Add awesome client side reporting stuff

  [Concern("Console runner")]
  public class when_arguments_are_not_provided
    : with_runner 
  {
    Because of = ()=>
      program.Run(new string[] {});

    It should_print_usage_statement = ()=>
      console.Lines.ShouldContain(Resources.UsageStatement);
  }

  [Concern("Console runner")]
  public class when_running_a_specification_assembly
    : with_runner 
  {
    Because of = ()=>
      program.Run(new [] {"Machine.Specifications.Example.dll"});

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
      console.ShouldContainLineWith("Contexts: 2");

    It should_write_the_count_of_specifications = ()=>
      console.ShouldContainLineWith("Specifications: 3");
  }

  [Concern("Console runner")]
  public class when_specifying_a_missing_assembly_on_the_command_line
    : with_runner 
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

  [Concern("Console runner")]
  public class when_a_specification_fails : with_runner
  {
    public static ExitCode exitCode;
    const string assemblyWithFailingSpecification = "Machine.Specifications.FailingExample";
    const string failingSpecificationName = "should fail";

    Because of = ()=>
      exitCode = program.Run(new string[] {assemblyWithFailingSpecification + ".dll"});

    It should_write_the_failure = ()=>
      console.ShouldContainLineWith("Exception");

    It should_write_the_count_of_failed_specifications = ()=>
      console.ShouldContainLineWith("1 failed");

    It should_return_the_Failure_exit_code = ()=>
      exitCode.ShouldEqual(ExitCode.Failure);
  }

  [Concern("Console runner")]
  public class when_the_user_specifies_an_html_path
    : with_runner
  {

    public static Options options;
    public static List<ISpecificationRunListener> listeners;
    public static bool result = false;

    Establish context = () =>
    {
      options = new Options();
      listeners = new List<ISpecificationRunListener>();

    };
  }

  [Concern("Console runner")]
  public class when_the_html_path_is_empty
    : when_the_user_specifies_an_html_path
  {
    Establish context = () =>
    {
      options.HtmlPath = string.Empty;
    };

    Because of = () =>
    {
      result = program.IsHtmlPathUnspecifiedOrSpecifiedAndValid(options, listeners);
    };

    It should_return_true_from_the_validation_method = () =>
    {
      result.ShouldBeTrue();
    };

    It should_have_not_added_the_ReportingRunListener_to_listeners = () =>
    {
      listeners.Count.Equals(0).ShouldBeTrue();
    };
  }

  [Concern("HTML command line argument")]
  public class when_the_html_path_is_invalid
    : when_the_user_specifies_an_html_path
  {
    Establish context = () =>
    {
      options.HtmlPath = @"if this is a valid path, you've managed to surprise me :)";
    };

    Because of = () =>
    {
      result = program.IsHtmlPathUnspecifiedOrSpecifiedAndValid(options, listeners);
    };

    It should_return_false_from_the_validation_method = () =>
    {
      result.ShouldBeFalse();
    };

    It should_have_not_added_the_ReportingRunListener_to_listeners = () =>
    {
      listeners.Count.Equals(0).ShouldBeTrue();
    };

  }

  [Concern("HTML command line argument")]
  public class when_the_html_path_is_valid
    : when_the_user_specifies_an_html_path
  {
    Establish context = () =>
    {
      options.HtmlPath = @"C:\";  // how to make this not fail on a loonix env?
                                  // i guess that's a code smell that the piece
                                  // that does file system checking should extracted
                                  // and mocked for the test
    };

    Because of = () =>
    {
      result = program.IsHtmlPathUnspecifiedOrSpecifiedAndValid(options, listeners);
    };

    It should_return_true_from_the_validation_method = () =>
    {
      result.ShouldBeFalse();
    };

    It should_have_added_the_ReprtingRunListener_to_listeners = () =>
    {
      listeners.Count.Equals(1).ShouldBeTrue();
    };

  }

  public class with_runner
  {
    public static Program program;
    public static FakeConsole console;

    Establish context = ()=>
    {
      console = new FakeConsole();
      program = new Program(console);
    };
  }
}