namespace Machine.Specifications.Comparers
{
    internal interface IEqualityStrategy<in T>
    {
        bool? Equals(T? x, T? y);
    }
}
