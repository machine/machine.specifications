using System;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobHandlerLocator
  {
    IBackgroundJobHandler LocateJobHandler(Type jobType);
  }
}