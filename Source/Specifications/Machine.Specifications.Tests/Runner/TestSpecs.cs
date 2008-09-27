using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  public class context_with_ignore_on_one_spec
  {
    public static bool IgnoredSpecRan;

    [Ignore]
    It should_be_ignored = () =>
      IgnoredSpecRan = true;
  }

  public class context_with_no_specs
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
}
