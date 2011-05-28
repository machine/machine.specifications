using System;

namespace Machine.Specifications
{
    class EquatableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            // Implements IEquatable<T>?
            var equatable = x as IEquatable<T>;

            if (equatable != null)
                return new ComparisionResult(equatable.Equals(y) ? 0 : -1);
            return new NoResult();
        }
    }
}