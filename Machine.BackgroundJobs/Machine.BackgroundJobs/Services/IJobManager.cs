using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobManager
  {
    IList<IBackgroundJob> FindCompletedJobs(Type jobType);
    IList<IBackgroundJob> FindActiveJobs(Type jobType);
    IList<IBackgroundJob> FindJobs(Type jobType);
    IBackgroundJob FindJob(Type jobType, Int32 jobNumber);
    IList<TJobType> FindCompletedJobs<TJobType>() where TJobType : IBackgroundJob;
    IList<TJobType> FindActiveJobs<TJobType>() where TJobType : IBackgroundJob;
    IList<TJobType> FindJobs<TJobType>() where TJobType : IBackgroundJob;
    TJobType FindJob<TJobType>(Int32 jobNumber) where TJobType : IBackgroundJob;
    void QueueJob(IBackgroundJob job);
    void CancelJob(IBackgroundJob job);
    void StartJob(IBackgroundJob job);
  }
}
