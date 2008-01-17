using System;
using System.Collections.Generic;
using System.Threading;

using Machine.Core.Utility;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class SimpleJobRepositoryLocator : IJobRepositoryLocator
  {
    #region Member Data
    private readonly Dictionary<Type, IJobRepository> _repositories = new Dictionary<Type, IJobRepository>();
    private readonly ReaderWriterLock _lock = new ReaderWriterLock();
    #endregion

    #region IJobRepositoryLocator Members
    public IJobRepository LocateJobRepository(Type jobType)
    {
      using (RWLock.AsReader(_lock))
      {
        if (_repositories.ContainsKey(jobType))
        {
          return _repositories[jobType];
        }
        _lock.UpgradeToWriterLock(Timeout.Infinite);
        _repositories[jobType] = new SimpleJobRepository();
        return _repositories[jobType];
      }
    }
    #endregion
  }
}