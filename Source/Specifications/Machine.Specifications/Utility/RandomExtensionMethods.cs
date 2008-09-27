using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Utility
{
  public static class RandomExtensionMethods
  {
    public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var t in enumerable)
      {
        action(t);
      }
    }

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

    public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
    {
      return attributeProvider.GetCustomAttributes(typeof(TAttribute), true).Any();
    }
  }
}