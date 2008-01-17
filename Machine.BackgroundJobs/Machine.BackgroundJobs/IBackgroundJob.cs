using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Services;

namespace Machine.BackgroundJobs
{
  public interface IBackgroundJob
  {
    Int32 JobNumber
    {
      get;
      set;
    }
    bool IsComplete
    {
      get;
      set;
    }
    bool IsStarted
    {
      get;
      set;
    }
    double PercentageComplete
    {
      get;
      set;
    }
    string StatusMessage
    {
      get;
      set;
    }
  }
}
