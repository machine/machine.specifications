namespace Machine.Specifications.Comparers
{
    internal class NullStrategy<T> : IEqualityStrategy<T>
    {
        public bool? Equals(T? x, T? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return null;
        }
    }
}
