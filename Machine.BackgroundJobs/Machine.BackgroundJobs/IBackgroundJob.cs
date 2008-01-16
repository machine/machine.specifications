using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs
{
  public interface IBackgroundJob
  {
    Int32 JobNumber
    {
      get;
    }
    bool IsComplete
    {
      get;
    }
    bool IsStarted
    {
      get;
    }
    double PercentageComplete
    {
      get;
    }
    string StatusMessage
    {
      get;
    }
  }
}
