using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobServicesLocator
  {
    IJobRepository LocateRepository(IBackgroundJob job);
    IBackgroundJobHandler LocateJobHandler(IBackgroundJob job);
  }
}
