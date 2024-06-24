namespace Machine.Specifications.Comparers
{
    internal class TypeStrategy<T> : IEqualityStrategy<T>
    {
        public bool? Equals(T? x, T? y)
        {
            if (x == null || y == null)
            {
                return null;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return null;
        }
    }
}
