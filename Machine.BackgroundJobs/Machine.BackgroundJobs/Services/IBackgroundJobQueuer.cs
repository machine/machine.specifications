using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IBackgroundJobQueuer
  {
    IBackgroundJob QueueJob(IBackgroundJob job);
  }
}
