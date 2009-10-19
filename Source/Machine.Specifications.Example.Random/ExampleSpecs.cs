using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Specs
{
  public static class tag
  {
    public const string example = "example";
    public const string some_other_tag = "some other tag";
    public const string one_more_tag = "one more tag";
  }

  [SetupForEachSpecification, Tags(tag.example)]
  public class context_with_multiple_specifications_and_setup_for_each
  {
    public static int EstablishRunCount;
    public static int BecauseClauseRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [Tags(tag.example, "foobar")]
  public class context_with_multiple_specifications
  {
    public static int EstablishRunCount;
    public static int BecauseClauseRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [Tags(tag.example, tag.example)]
  [Tags(tag.example)]
  public class context_with_duplicate_tags
  {
    It bla_bla = () => { };
  }

  [Tags(tag.example, tag.some_other_tag, tag.one_more_tag)]
  public class context_with_tags
  {
    It bla_bla = () => { };
  }

  [Ignore]
  public class context_with_ignore : context_with_no_specs
  {
    public static bool IgnoredSpecRan;

    It should_be_ignored = () =>
      IgnoredSpecRan = true;
  }

  public class context_with_ignore_on_one_spec : context_with_no_specs
  {
    public static bool IgnoredSpecRan;

    [Ignore]
    It should_be_ignored = () =>
      IgnoredSpecRan = true;
  }

  [Tags(tag.example)]
  public class context_with_no_specs
  {
    public static bool ContextEstablished;
    public static bool CleanupOccurred;

    Establish context = () =>
    {
      ContextEstablished = true;
    };

    Cleanup after_each = () =>
    {
      CleanupOccurred = true;
    };
  }

  [Subject(typeof(int), "Some description")]
  [Tags(tag.example)]
  public class context_with_subject
  {
  }

  [Tags(tag.example)]
  public class context_with_failing_specs
  {
    It should = () => { throw new InvalidOperationException("something went wrong"); };
  }

  [Tags(tag.example)]
  public class context_with_failing_establish
  {
    Establish context = () => { throw new InvalidOperationException("something went wrong"); };
    It should = () => { };
  }

  [Tags(tag.example)]
  public class context_with_failing_because
  {
    Because of = () => { throw new InvalidOperationException("something went wrong"); };
    It should = () => { };
  }

  [Tags(tag.example)]
  public class context_with_console_output
  {
    Establish context = () =>
    {
      Console.Out.WriteLine("Console.Out message in establish");
      Console.Error.WriteLine("Console.Error message in establish");
    };

    Because of = () =>
    {
      Console.Out.WriteLine("Console.Out message in because");
      Console.Error.WriteLine("Console.Error message in because");
    };

    Cleanup after = () =>
    {
      Console.Out.WriteLine("Console.Out message in cleanup");
      Console.Error.WriteLine("Console.Error message in cleanup");
    };

    It should_log_messages = () =>
    {
      Console.Out.WriteLine("Console.Out message in spec");
      Console.Error.WriteLine("Console.Error message in spec");
    };

    It should_log_messages_also_for_the_nth_run = () =>
    {
      Console.Out.WriteLine("Console.Out message in spec");
      Console.Error.WriteLine("Console.Error message in spec");
    };
  }

  [Tags(tag.example)]
  public class context_with_inner_exception
  {
    It should_throw = () =>
    {
      try
      {
        throw new Exception("INNER123");

      }
      catch (Exception err)
      {
        throw new TargetInvocationException(err);
      }
    };
  }

  [SetupForEachSpecification, Tags(tag.example)]
  public class context_with_console_output_and_for_each
  {
    Establish context = () =>
    {
      Console.Out.WriteLine("Console.Out message in establish");
      Console.Error.WriteLine("Console.Error message in establish");
    };

    Because of = () =>
    {
      Console.Out.WriteLine("Console.Out message in because");
      Console.Error.WriteLine("Console.Error message in because");
    };

    Cleanup after_each = () =>
    {
      Console.Out.WriteLine("Console.Out message in cleanup");
      Console.Error.WriteLine("Console.Error message in cleanup");
    };

    It should_log_messages = () =>
    {
      Console.Out.WriteLine("Console.Out message in spec");
      Console.Error.WriteLine("Console.Error message in spec");
    };

    It should_log_messages_also_for_the_nth_run = () =>
    {
      Console.Out.WriteLine("Console.Out message in spec");
      Console.Error.WriteLine("Console.Error message in spec");
    };
  }
}
