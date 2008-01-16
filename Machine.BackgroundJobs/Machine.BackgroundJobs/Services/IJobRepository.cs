using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobRepository
  {
    ICollection<IBackgroundJob> GetPendingJobs();
    void AddJob(IBackgroundJob job);
    void SaveJob(IBackgroundJob job);
  }
}
