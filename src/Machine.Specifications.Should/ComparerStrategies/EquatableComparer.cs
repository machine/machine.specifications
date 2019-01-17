using System;

namespace Machine.Specifications.ComparerStrategies
{
    class EquatableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            var equatable = x as IEquatable<T>;

            if (equatable == null)
            {
                return new NoResult();
            }

            return new ComparisionResult(equatable.Equals(y) ? 0 : -1);
        }
    }
}