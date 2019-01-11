namespace Machine.Specifications.Reporting.Integration
{
    public interface ISpecificationResultProvider
    {
        bool FailureOccurred { get; }
    }
}