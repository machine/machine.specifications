using System;
using System.Collections.Generic;

namespace Machine.Specifications.Runner.Utility
{
    internal static class RandomExtensionMethods
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