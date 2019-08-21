using System;
using System.Reflection;

namespace Machine.Specifications.ComparerStrategies
{
    class ComparableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            var comparable1 = x as IComparable<T>;

            if (comparable1 != null)
            {
                return new ComparisionResult(comparable1.CompareTo(y));
            }

            var comparable2 = x as IComparable;

            if (comparable2 != null)
            {
                if (!(comparable2.GetType().IsInstanceOfType(y)))
                {
                    return new NoResult();
                }
                return new ComparisionResult(comparable2.CompareTo(y));
            }
            return new NoResult();
        }
    }
}