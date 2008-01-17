using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobRepository
  {
    IList<IBackgroundJob> FindCompletedJobs();
    IList<IBackgroundJob> FindActiveJobs();
    IList<IBackgroundJob> FindJobs();
    IBackgroundJob FindJob(Int32 jobNumber);
    void SaveJob(IBackgroundJob job);
  }
}
