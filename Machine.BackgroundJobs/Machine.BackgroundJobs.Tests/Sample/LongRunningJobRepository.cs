using System.Collections.Generic;

using Machine.BackgroundJobs.Services;

namespace Machine.BackgroundJobs.Sample
{
  public class LongRunningJobRepository : IJobRepository
  {
    #region IJobRepository Members
    public ICollection<IBackgroundJob> GetPendingJobs()
    {
      return new List<IBackgroundJob>();
    }

    public void AddJob(IBackgroundJob job)
    {
    }

    public void SaveJob(IBackgroundJob job)
    {
    }
    #endregion
  }
}