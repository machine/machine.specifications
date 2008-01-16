using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Services
{
  public interface IJobTypeRepository
  {
    void RegisterBackgroundJobType(Type type);
  }
}
