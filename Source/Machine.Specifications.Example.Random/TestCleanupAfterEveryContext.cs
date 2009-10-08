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
}