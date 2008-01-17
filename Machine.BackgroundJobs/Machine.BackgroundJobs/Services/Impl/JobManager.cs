using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class JobManager : IJobManager
  {
    #region Member Data
    private readonly IJobRepositoryLocator _jobRepositoryLocator;
    private readonly IBackgroundJobQueuer _backgroundJobQueuer;
    private readonly IBackgroundJobSpawner _backgroundJobSpawner;
    #endregion

    #region JobManager()
    public JobManager(IJobRepositoryLocator jobRepositoryLocator, IBackgroundJobQueuer backgroundJobQueuer, IBackgroundJobSpawner backgroundJobSpawner)
    {
      _backgroundJobSpawner = backgroundJobSpawner;
      _jobRepositoryLocator = jobRepositoryLocator;
      _backgroundJobQueuer = backgroundJobQueuer;
    }
    #endregion

    #region IJobManager Members
    public IList<IBackgroundJob> FindActiveJobs(Type jobType)
    {
      return _jobRepositoryLocator.LocateJobRepository(jobType).FindActiveJobs();
    }

    public IList<IBackgroundJob> FindCompletedJobs(Type jobType)
    {
      return _jobRepositoryLocator.LocateJobRepository(jobType).FindCompletedJobs();
    }

    public IList<IBackgroundJob> FindJobs(Type jobType)
    {
      return _jobRepositoryLocator.LocateJobRepository(jobType).FindJobs();
    }

    public IBackgroundJob FindJob(Type jobType, Int32 jobNumber)
    {
      return _jobRepositoryLocator.LocateJobRepository(jobType).FindJob(jobNumber);
    }

    public IList<TJobType> FindCompletedJobs<TJobType>() where TJobType : IBackgroundJob
    {
      return new List<TJobType>(Algorithms.TypedAs<TJobType>(FindCompletedJobs(typeof(TJobType))));
    }

    public IList<TJobType> FindActiveJobs<TJobType>() where TJobType : IBackgroundJob
    {
      return new List<TJobType>(Algorithms.TypedAs<TJobType>(FindActiveJobs(typeof(TJobType))));
    }

    public IList<TJobType> FindJobs<TJobType>() where TJobType : IBackgroundJob
    {
      return new List<TJobType>(Algorithms.TypedAs<TJobType>(FindJobs(typeof(TJobType))));
    }

    public TJobType FindJob<TJobType>(Int32 jobNumber) where TJobType : IBackgroundJob
    {
      return (TJobType)FindJob(typeof(TJobType), jobNumber);
    }

    public void QueueJob(IBackgroundJob job)
    {
      _backgroundJobQueuer.QueueJob(job);
    }

    public void CancelJob(IBackgroundJob job)
    {
    }

    public void StartJob(IBackgroundJob job)
    {
      _backgroundJobQueuer.QueueJob(job);
      _backgroundJobSpawner.SpawnJob(job);
    }
    #endregion
  }
}
