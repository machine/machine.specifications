using System.Collections.Generic;

namespace Machine.Specifications.ComparerStrategies
{
    class DefaultComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            // Last case, rely on Object.Equals
            return Equals(x, y) ? 0 : -1;
        }
    }
}