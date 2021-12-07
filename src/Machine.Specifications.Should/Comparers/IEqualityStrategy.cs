namespace Machine.Specifications.Comparers
{
    internal interface IEqualityStrategy<T>
    {
        bool? Equals(T? x, T? y);
    }
}
