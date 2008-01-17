using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class BackgroundJobQueuer : IBackgroundJobQueuer
  {
    #region Member Data
    private readonly IJobRepositoryLocator _jobRepositoryLocator;
    #endregion

    #region BackgroundJobQueuer()
    public BackgroundJobQueuer(IJobRepositoryLocator jobRepositoryLocator)
    {
      _jobRepositoryLocator = jobRepositoryLocator;
    }
    #endregion

    #region IJobQueuer Members
    public IBackgroundJob QueueJob(IBackgroundJob job)
    {
      IJobRepository jobRepository = _jobRepositoryLocator.LocateJobRepository(job.GetType());
      jobRepository.SaveJob(job);
      return job;
    }
    #endregion
  }
}
