using System;

namespace Machine.Specifications.Specs
{
  public class TestCleanupAfterEveryContext : ICleanupAfterEveryContextInAssembly
  {
    public static bool AfterContextCleanupRun;
    public static int AfterContextCleanupRunCount;

    public void AfterContextCleanup()
    {
      ++AfterContextCleanupRunCount;
      AfterContextCleanupRun = true;
    }

    public static void Reset()
    {
      AfterContextCleanupRun = false;
      AfterContextCleanupRunCount = 0;
    }
  }

  [Tags("foobar")]
  public class TaggedCleanup : ICleanupAfterEveryContextInAssembly
  {
    public static bool AfterContextCleanupRun;
    public static int AfterContextCleanupRunCount;

    public void AfterContextCleanup()
    {
      ++AfterContextCleanupRunCount;
      AfterContextCleanupRun = true;
    }

    public static void Reset()
    {
      AfterContextCleanupRun = false;
      AfterContextCleanupRunCount = 0;
    }
  }

  public class UntaggedCleanup : ICleanupAfterEveryContextInAssembly
  {
    public static bool AfterContextCleanupRun;
    public static int AfterContextCleanupRunCount;

    public void AfterContextCleanup()
    {
      ++AfterContextCleanupRunCount;
      AfterContextCleanupRun = true;
    }

    public static void Reset()
    {
      AfterContextCleanupRun = false;
      AfterContextCleanupRunCount = 0;
    }
  }

  [Tags("foobar")]
  public class TaggedAssemblyContext : IAssemblyContext
  {
    public static bool OnAssemblyStartRun;
    public static bool OnAssemblyCompleteRun;

    public static void Reset()
    {
      OnAssemblyStartRun = false;
      OnAssemblyCompleteRun = false;
    }

    public void OnAssemblyStart()
    {
      OnAssemblyStartRun = true;
    }

    public void OnAssemblyComplete()
    {
      OnAssemblyCompleteRun = true;
    }
  }

  public class UntaggedAssemblyContext : IAssemblyContext
  {
    public static bool OnAssemblyStartRun;
    public static bool OnAssemblyCompleteRun;

    public static void Reset()
    {
      OnAssemblyStartRun = false;
      OnAssemblyCompleteRun = false;
    }

    public void OnAssemblyStart()
    {
      OnAssemblyStartRun = true;
    }

    public void OnAssemblyComplete()
    {
      OnAssemblyCompleteRun = true;
    }
  }
}