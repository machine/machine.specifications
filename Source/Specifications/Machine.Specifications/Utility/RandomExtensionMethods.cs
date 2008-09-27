using System;
using System.Collections.Generic;

namespace Machine.Specifications.Utility
{
  public static class RandomExtensionMethods
  {
    public static void InvokeIfNotNull(this Because because)
    {
      if (because != null)
        because.Invoke();
    }

    public static void InvokeAll(this IEnumerable<Establish> contextActions)
    {
      foreach (Establish contextAction in contextActions)
      {
        contextAction();
      }
    }

    public static void InvokeAll(this IEnumerable<Cleanup> contextActions)
    {
      foreach (Cleanup contextAction in contextActions)
      {
        contextAction();
      }
    }
  }
}