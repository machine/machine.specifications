using Machine.Specifications.Reporting.Model;

using Spark;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public abstract class SparkView : AbstractSparkView
  {
    public Run Model
    {
      get;
      set;
    }
  }
}