namespace Machine.Specifications.Runner.Utility
{
    internal struct HashCode
    {
        private readonly int value;

        private HashCode(int value)
        {
            this.value = value;
        }

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.value;
        }

        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }

        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(value, GetHashCode(item)));
        }

        private static int GetHashCode<T>(T item)
        {
            return item?.GetHashCode() ?? 0;
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            unchecked
            {
                return ((h1 << 5) + h1) ^ h2;
            }
        }
    }
}
