namespace Machine.Specifications.ComparerStrategies
{
    interface IComparerStrategy<T>
    {
        ComparisionResult Compare(T x, T y);
    }
}