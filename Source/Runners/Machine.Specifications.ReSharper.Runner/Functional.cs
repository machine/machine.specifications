using System;

namespace Machine.Specifications.ReSharper.Runner
{
    static class Functional
    {
        public static Func<T> Memoize<T>(Func<T> func)
        {
            var t = default(T);
            var hasValue = false;
            return () =>
            {
                if (!hasValue)
                {
                    t = func();
                    hasValue = true;
                }
                return t;
            };
        }
    }
}