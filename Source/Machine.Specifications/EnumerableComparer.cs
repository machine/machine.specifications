using System.Collections;

namespace Machine.Specifications
{
    class EnumerableComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            // Enumerable?
            var enumerableX = x as IEnumerable;
            var enumerableY = y as IEnumerable;

            if (enumerableX != null && enumerableY != null)
            {
                IEnumerator enumeratorX = enumerableX.GetEnumerator();
                IEnumerator enumeratorY = enumerableY.GetEnumerator();

                while (true)
                {
                    bool hasNextX = enumeratorX.MoveNext();
                    bool hasNextY = enumeratorY.MoveNext();

                    if (!hasNextX || !hasNextY)
                        return new ComparisionResult(hasNextX == hasNextY ? 0 : -1);

                    if (!object.Equals(enumeratorX.Current, enumeratorY.Current))
                        return new ComparisionResult(-1);
                }
            }
            return new NoResult();
        }
    }
}