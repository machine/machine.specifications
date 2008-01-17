using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class JobManager : IJobManager
  {
    #region Member Data
    private readonly IJobServicesLocator _jobServicesLocator;
    private readonly IBackgroundJobQueuer _backgroundJobQueuer;
    private readonly IBackgroundJobSpawner _backgroundJobSpawner;
    #endregion

    #region JobManager()
    public JobManager(IJobServicesLocator jobServicesLocator, IBackgroundJobQueuer backgroundJobQueuer, IBackgroundJobSpawner backgroundJobSpawner)
    {
      _jobServicesLocator = jobServicesLocator;
      _backgroundJobSpawner = backgroundJobSpawner;
      _backgroundJobQueuer = backgroundJobQueuer;
    }
    #endregion

    #region IJobManager Members
    public IList<IBackgroundJob> GetActiveJobs(Type jobType)
    {
      return _jobServicesLocator.LocateRepository(jobType).GetActiveJobs();
    }

    public IList<IBackgroundJob> GetCompletedJobs(Type jobType)
    {
      return _jobServicesLocator.LocateRepository(jobType).GetCompletedJobs();
    }

    public IBackgroundJob FindJob(Type jobType, Int32 jobNumber)
    {
      return _jobServicesLocator.LocateRepository(jobType).FindJob(jobNumber);
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
      _backgroundJobSpawner.SpawnJob(job);
    }
    #endregion
  }
}
