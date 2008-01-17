using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobServicesLocator
  {
    IJobRepository LocateRepository(Type jobType);
    IBackgroundJobHandler LocateJobHandler(Type jobType);
  }
}
