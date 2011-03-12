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

    internal static void InvokeAll(this IEnumerable<Establish> contextActions)
    {
      contextActions.AllNonNull().Select<Establish, Action>(x => x.Invoke).InvokeAll();
    }

    static IEnumerable<T> AllNonNull<T>(this IEnumerable<T> elements) where T : class
    {
      return elements.Where(x => x != null);
    }

    static void InvokeAll(this IEnumerable<Action> actions)
    {
      foreach (Action action in actions)
      {
        action();
      }
    }

    internal static void InvokeAll(this IEnumerable<Because> becauseActions)
    {
      becauseActions.AllNonNull().Select<Because, Action>(x => x.Invoke).InvokeAll();
    }

    internal static void InvokeAll(this IEnumerable<Cleanup> contextActions)
    {
      contextActions.AllNonNull().Select<Cleanup, Action>(x => x.Invoke).InvokeAll();
    }

    internal static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
    {
      return attributeProvider.GetCustomAttributes(typeof(TAttribute), true).Any();
    }
  }
}