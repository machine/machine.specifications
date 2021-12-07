using System.Collections.Generic;

namespace Machine.Specifications.Comparers
{
    internal class AssertEqualityComparer<T> : IEqualityComparer<T>
    {
        private static readonly IEqualityComparerStrategy<T>[] Comparers = new IEqualityComparerStrategy<T>[]
        {
            new EquatableComparer<T>(),
            new ComparableComparer<T>(),
            new EnumerableComparer<T>(),
            new GenericTypeComparer<T>(),
            new TypeComparer<T>()
        };

        private static readonly DefaultComparer<T> DefaultComparer = new DefaultComparer<T>();

        public static readonly AssertEqualityComparer<T> Default = new AssertEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            foreach (var comparer in Comparers)
            {
                var result = comparer.Equals(x, y);

                if (result != null)
                {
                    return result.Value;
                }
            }

            return DefaultComparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
