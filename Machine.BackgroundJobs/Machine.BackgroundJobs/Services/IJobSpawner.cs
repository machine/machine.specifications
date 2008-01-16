using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobSpawner
  {
    void Start();
    void Stop();
  }
}
