using System;
using System.Web;

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

    public string H(object value)
    {
      return HttpUtility.HtmlEncode(Convert.ToString(value));
    }
  }
}