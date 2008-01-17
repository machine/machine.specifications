using System;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobRepositoryLocator
  {
    IJobRepository LocateJobRepository(Type jobType);
  }
}