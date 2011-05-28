namespace Machine.Specifications
{
    public interface IComparerStrategy<T>
    {
        ComparisionResult Compare(T x, T y);
    }
}