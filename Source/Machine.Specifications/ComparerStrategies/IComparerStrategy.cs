namespace Machine.Specifications.ComparerStrategies
{
    public interface IComparerStrategy<T>
    {
        ComparisionResult Compare(T x, T y);
    }
}