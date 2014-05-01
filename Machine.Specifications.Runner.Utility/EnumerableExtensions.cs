using System;
using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
            {
                action(t);
            }
        }
    }
}