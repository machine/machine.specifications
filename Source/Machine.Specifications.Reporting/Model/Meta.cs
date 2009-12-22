using System;

namespace Machine.Specifications.Reporting.Model
{
  public class Meta
  {
    public DateTime GeneratedAt
    {
      get;
      set;
    }

    public bool ShouldGenerateTimeInfo
    {
      get;
      set;
    }

    public bool ShouldGenerateIndexLink
    {
      get;
      set;
    }

    public string IndexLink
    {
      get;
      set;
    }
  }
}