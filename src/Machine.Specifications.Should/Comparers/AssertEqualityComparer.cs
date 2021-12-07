using System.Collections.Generic;

namespace Machine.Specifications.Comparers
{
    internal class AssertEqualityComparer<T> : IEqualityComparer<T>
    {
        private static IEqualityStrategy<T>[] Strategies = new IEqualityStrategy<T>[]
        {
            new NullStrategy<T>(),
            new EquatableStrategy<T>(),
            new ComparableStrategy<T>(),
            new EnumerableStrategy<T>(),
            new TypeStrategy<T>(),
            new GenericTypeStrategy<T>()
        };

        public static readonly AssertEqualityComparer<T> Default = new AssertEqualityComparer<T>();

        public bool Equals(T? x, T? y)
        {
            foreach (var strategy in Strategies)
            {
                var result = strategy.Equals(x, y);

                if (result != null)
                {
                    return result.Value;
                }
            }

            return object.Equals(x, y);
        }

        public int GetHashCode(T? obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
