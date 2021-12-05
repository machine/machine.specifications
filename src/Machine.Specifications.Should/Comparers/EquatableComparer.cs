using System;

namespace Machine.Specifications.Comparers
{
    internal class EquatableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            if (!(x is IEquatable<T> equatable))
            {
                return new NoResult();
            }

            return new ComparisionResult(equatable.Equals(y) ? 0 : -1);
        }
    }
}
