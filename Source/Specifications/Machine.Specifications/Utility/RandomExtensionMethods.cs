using System.Collections.Generic;

namespace Machine.Specifications.Utility
{
  public static class RandomExtensionMethods
  {
    public static void InvokeAll(this IEnumerable<Context> contextActions)
    {
      foreach (Context contextAction in contextActions)
      {
        contextAction();
      }
    }
  }
}