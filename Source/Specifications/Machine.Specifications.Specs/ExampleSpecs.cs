using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Specs
{
  public class context_with_duplicate_tags : Tags<example>, Tags<example, example>
  {
    It bla_bla = ()=> { };
  }

  public class context_with_tags : Tags<example>, Tags<some_other_tag, one_more_tag>
  {
    It bla_bla = ()=> { };
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

  public class context_with_no_specs : Tags<example>
  {
    public static bool ContextEstablished;
    public static bool OneTimeContextEstablished;
    public static bool CleanupOccurred;
    public static bool CleanupOnceOccurred;

    Establish context = () =>
    {
      ContextEstablished = true;
    };

    Establish context_once = () =>
    {
      ContextEstablished = true;
    };

    Cleanup after_each = () =>
    {
      CleanupOccurred = true;
    };

    Cleanup after_all = () =>
    {
      CleanupOnceOccurred = true;
    };
  }

  [Subject(typeof(int), "Some description")]
  public class context_with_subject : Tags<example>
  {
    public void Reset()
    {
    }
  }
}
