using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IBackgroundJobHandler
  {
    bool CanHandleJob();
    void HandleJob(IBackgroundJob job);
  }
}
