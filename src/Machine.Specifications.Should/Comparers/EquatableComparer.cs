using System;

namespace Machine.Specifications.Comparers
{
    internal class EquatableComparer<T> : IEqualityComparerStrategy<T>
    {
        public bool? Equals(T x, T y)
        {
            if (!(x is IEquatable<T> equatable))
            {
                return null;
            }

            return equatable.Equals(y);
        }
    }
}
