using System.Collections;

namespace Machine.Specifications.ComparerStrategies
{
    internal class EnumerableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
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
                        return new ComparisionResult(hasNextX == hasNextY ? 0 : -1);
                    }

                    if (!Equals(enumeratorX.Current, enumeratorY.Current))
                    {
                        return new ComparisionResult(-1);
                    }
                }
            }

            return new NoResult();
        }
    }
}
