using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class JobQueuer : IJobQueuer
  {
    #region IJobQueuer Members
    public IBackgroundJob QueueJob(IBackgroundJob job)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
