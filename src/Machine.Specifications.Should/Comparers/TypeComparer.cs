namespace Machine.Specifications.Comparers
{
    internal class TypeComparer<T> : IEqualityComparerStrategy<T>
    {
        public bool? Equals(T x, T y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return null;
        }
    }
}
