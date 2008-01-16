using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobServicesSelector
  {
    IJobRepository CreateRepository(IBackgroundJob job);
    IBackgroundJobHandler CreateJobHandler(IBackgroundJob job);
  }
}
