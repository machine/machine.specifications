using System;

namespace Machine.Specifications.Comparers
{
    internal class ComparableComparer<T> : IEqualityComparerStrategy<T>
    {
        public bool? Equals(T x, T y)
        {
            if (x is IComparable<T> comparable1)
            {
                return comparable1.CompareTo(y) == 0;
            }

            if (x is IComparable comparable2)
            {
                if (!comparable2.GetType().IsInstanceOfType(y))
                {
                    return null;
                }

                return comparable2.CompareTo(y) == 0;
            }

            return null;
        }
    }
}
