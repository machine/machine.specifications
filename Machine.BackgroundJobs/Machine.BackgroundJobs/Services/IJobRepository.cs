using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobRepository
  {
    IList<IBackgroundJob> GetCompletedJobs();
    IList<IBackgroundJob> GetActiveJobs();
    IBackgroundJob FindJob(Int32 jobNumber);
    void AddJob(IBackgroundJob job);
    void SaveJob(IBackgroundJob job);
  }
}
