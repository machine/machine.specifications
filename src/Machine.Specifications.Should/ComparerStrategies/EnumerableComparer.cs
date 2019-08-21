using System.Collections;

namespace Machine.Specifications.ComparerStrategies
{
    class EnumerableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            var enumerableX = x as IEnumerable;
            var enumerableY = y as IEnumerable;

            if (enumerableX != null && enumerableY != null)
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