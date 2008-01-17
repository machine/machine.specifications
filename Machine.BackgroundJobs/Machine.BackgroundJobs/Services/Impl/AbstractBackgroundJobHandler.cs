using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services.Impl
{
  public abstract class AbstractBackgroundJobHandler<TJobType> : IBackgroundJobHandler where TJobType : class, IBackgroundJob
  {
    #region IBackgroundJobHandler Members
    public void HandleJob(IBackgroundJob job)
    {
      TJobType ourJob = job as TJobType;
      if (ourJob == null)
      {
        throw new ArgumentException("job");
      }
      HandleJob(ourJob);
    }

    public abstract void HandleJob(TJobType job);
    #endregion
  }
}
