namespace Machine.Specifications.Comparers
{
    internal interface IComparerStrategy<in T>
    {
        ComparisionResult Compare(T x, T y);
    }
}
