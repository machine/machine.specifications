using System;
using System.Collections.Generic;

using Machine.Core.Services;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class BackgroundJobSpawner : IBackgroundJobSpawner
  {
    #region Member Data
    private readonly IThreadManager _threadManager;
    private readonly IJobServicesLocator _jobServicesLocator;
    #endregion

    #region JobSpawner()
    public BackgroundJobSpawner(IThreadManager threadManager, IJobServicesLocator jobServicesLocator)
    {
      _threadManager = threadManager;
      _jobServicesLocator = jobServicesLocator;
    }
    #endregion

    #region IJobSpawner Members
    public void SpawnJob(IBackgroundJob job)
    {
      IBackgroundJobHandler jobHandler = _jobServicesLocator.LocateJobHandler(job.GetType());
      _threadManager.CreateThread(new QueuedJob(job, jobHandler));
    }
    #endregion
  }
}
