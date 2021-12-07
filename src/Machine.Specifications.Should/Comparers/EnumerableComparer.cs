using System.Collections;

namespace Machine.Specifications.Comparers
{
    internal class EnumerableComparer<T> : IEqualityComparerStrategy<T>
    {
        public bool? Equals(T x, T y)
        {
            if (x is IEnumerable enumerableX && y is IEnumerable enumerableY)
            {
                var enumeratorX = enumerableX.GetEnumerator();
                var enumeratorY = enumerableY.GetEnumerator();

                while (true)
                {
                    var hasNextX = enumeratorX.MoveNext();
                    var hasNextY = enumeratorY.MoveNext();

                    if (!hasNextX || !hasNextY)
                    {
                        return hasNextX == hasNextY;
                    }

                    if (!Equals(enumeratorX.Current, enumeratorY.Current))
                    {
                        return false;
                    }
                }
            }

            return null;
        }
    }
}
