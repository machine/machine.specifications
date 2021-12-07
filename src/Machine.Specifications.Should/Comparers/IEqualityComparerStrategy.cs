namespace Machine.Specifications.Comparers
{
    internal interface IEqualityComparerStrategy<T>
    {
        bool? Equals(T x, T y);
    }
}
