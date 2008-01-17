using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Sample
{
  [BackgroundJob(typeof(LongRunningJobHandler), typeof(LongRunningJobRepository))]
  public class LongRunningJob : SimpleBackgroundJob
  {
    private TimeSpan _runFor;

    public TimeSpan RunFor
    {
      get { return _runFor; }
      set { _runFor = value; }
    }
  }
}
