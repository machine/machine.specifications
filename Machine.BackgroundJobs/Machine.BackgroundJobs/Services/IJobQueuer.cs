using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobQueuer
  {
    IBackgroundJob QueueJob(IBackgroundJob job);
  }
}
