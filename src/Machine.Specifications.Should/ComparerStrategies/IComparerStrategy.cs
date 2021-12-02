namespace Machine.Specifications.ComparerStrategies
{
    internal interface IComparerStrategy<in T>
    {
        ComparisionResult Compare(T x, T y);
    }
}
