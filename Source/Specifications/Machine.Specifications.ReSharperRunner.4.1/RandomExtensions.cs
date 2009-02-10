using System;
using System.Collections.Generic;

namespace Machine.Specifications.ReSharperRunner
{
  internal static class RandomExtensions
  {
    public static IEnumerable<TSource> Flatten<TSource>(this TSource parent,
                                                        Func<TSource, IEnumerable<TSource>> childSelector)
    {
      foreach (TSource node in childSelector(parent))
      {
        yield return node;

        foreach (TSource descendant in childSelector(node))
        {
          yield return descendant;
        }
      }
    }
  }
}