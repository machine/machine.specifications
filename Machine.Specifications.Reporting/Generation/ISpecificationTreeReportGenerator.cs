using Machine.Specifications.Reporting.Model;

namespace Machine.Specifications.Reporting.Generation
{
    public interface ISpecificationTreeReportGenerator
    {
        void GenerateReport(Run run);
    }
}