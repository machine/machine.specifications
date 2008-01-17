using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IBackgroundJobHandler
  {
    void HandleJob(IBackgroundJob job);
  }
}
