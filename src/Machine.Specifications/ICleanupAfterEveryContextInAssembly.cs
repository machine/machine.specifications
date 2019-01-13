namespace Machine.Specifications
{
    public interface ICleanupAfterEveryContextInAssembly
    {
        void AfterContextCleanup();
    }

    public interface ISupplementSpecificationResults
    {
        Result SupplementResult(Result result);
    }
}