using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobManager
  {
    IList<IBackgroundJob> GetCompletedJobs(Type jobType);
    IList<IBackgroundJob> GetActiveJobs(Type jobType);
    IBackgroundJob FindJob(Type jobType, Int32 jobNumber);
    TJobType FindJob<TJobType>(Int32 jobNumber) where TJobType : IBackgroundJob;
    void QueueJob(IBackgroundJob job);
    void CancelJob(IBackgroundJob job);
    void StartJob(IBackgroundJob job);
  }
}
