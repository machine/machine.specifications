using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class BackgroundJobQueuer : IBackgroundJobQueuer
  {
    #region Member Data
    private readonly IJobServicesLocator _jobServicesLocator;
    #endregion

    #region BackgroundJobQueuer()
    public BackgroundJobQueuer(IJobServicesLocator jobServicesLocator)
    {
      _jobServicesLocator = jobServicesLocator;
    }
    #endregion

    #region IJobQueuer Members
    public IBackgroundJob QueueJob(IBackgroundJob job)
    {
      IJobRepository jobRepository = _jobServicesLocator.LocateRepository(job.GetType());
      jobRepository.SaveJob(job);
      return job;
    }
    #endregion
  }
}
