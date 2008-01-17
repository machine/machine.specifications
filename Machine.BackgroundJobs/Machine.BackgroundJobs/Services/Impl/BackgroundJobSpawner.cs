using System;
using System.Collections.Generic;

using Machine.Core.Services;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class BackgroundJobSpawner : IBackgroundJobSpawner
  {
    #region Member Data
    private readonly IThreadManager _threadManager;
    private readonly IJobHandlerLocator _jobHandlerLocator;
    #endregion

    #region JobSpawner()
    public BackgroundJobSpawner(IThreadManager threadManager, IJobHandlerLocator jobHandlerLocator)
    {
      _threadManager = threadManager;
      _jobHandlerLocator = jobHandlerLocator;
    }
    #endregion

    #region IJobSpawner Members
    public void SpawnJob(IBackgroundJob job)
    {
      IBackgroundJobHandler jobHandler = _jobHandlerLocator.LocateJobHandler(job.GetType());
      _threadManager.CreateThread(new QueuedJob(job, jobHandler));
    }
    #endregion
  }
}
