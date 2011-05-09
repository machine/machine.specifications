namespace Machine.Specifications
{
    class TypeComparer<T> : IComparerStrategy<T>
    {
        public ComparisionResult Compare(T x, T y)
        {
            if (x.GetType() != y.GetType())
                return new ComparisionResult(-1);
            return new NoResult();
        }
    }
}