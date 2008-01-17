using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IBackgroundJobSpawner
  {
    void SpawnJob(IBackgroundJob job);
  }
}
