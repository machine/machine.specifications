using System;

namespace Machine.Specifications.Comparers
{
    internal class GenericTypeStrategy<T> : IEqualityStrategy<T>
    {
        public bool? Equals(T? x, T? y)
        {
            var type = typeof(T);

            if (!type.IsValueType || IsNullableGeneric(type))
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

        private bool IsNullableGeneric(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>));
        }
    }
}
