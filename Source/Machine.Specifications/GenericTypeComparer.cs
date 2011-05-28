using System;

namespace Machine.Specifications
{
    class GenericTypeComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            Type type = typeof(T);
            // Null?
            if (!type.IsValueType || (type.IsGenericType && type.IsNullable()))
            {
                if (x.IsEqualToDefault())
                {
                    return new ComparisionResult(y.IsEqualToDefault() ? 0 : -1);
                }

                if (y.IsEqualToDefault())
                    return new ComparisionResult(-1);
            }
            return new NoResult();
        }

    }
}