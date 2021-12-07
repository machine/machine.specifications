using System;

namespace Machine.Specifications.Comparers
{
    internal class GenericTypeComparer<T> : IEqualityComparerStrategy<T>
    {
        public bool? Equals(T x, T y)
        {
            var type = typeof(T);

            if (!type.IsValueType || (type.IsGenericType && IsNullable(type)))
            {
                if (object.Equals(x, default(T)))
                {
                    return object.Equals(y, default(T));
                }

                if (object.Equals(y, default(T)))
                {
                    return false;
                }
            }

            return null;
        }

        private bool IsNullable(Type type)
        {
            return type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
        }
    }
}
