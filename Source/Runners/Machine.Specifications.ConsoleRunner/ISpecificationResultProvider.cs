namespace Machine.Specifications.ConsoleRunner
{
    public interface ISpecificationResultProvider
    {
        bool FailureOccurred { get; }
    }
}