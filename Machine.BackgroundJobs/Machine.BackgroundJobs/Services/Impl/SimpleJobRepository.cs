using System;
using System.Collections.Generic;
using System.Threading;

using Machine.Core.Utility;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class SimpleJobRepository : IJobRepository
  {
    #region Member Data
    private readonly ReaderWriterLock _lock = new ReaderWriterLock();
    private readonly List<IBackgroundJob> _jobs = new List<IBackgroundJob>();
    #endregion

    #region IJobRepository Members
    public virtual IList<IBackgroundJob> GetCompletedJobs()
    {
      using (RWLock.AsWriter(_lock))
      {
        List<IBackgroundJob> filtered = new List<IBackgroundJob>();
        foreach (IBackgroundJob job in _jobs)
        {
          if (job.IsComplete)
          {
            filtered.Add(job);
          }
        }
        return filtered;
      }
    }

    public virtual IList<IBackgroundJob> GetActiveJobs()
    {
      using (RWLock.AsWriter(_lock))
      {
        List<IBackgroundJob> filtered = new List<IBackgroundJob>();
        foreach (IBackgroundJob job in _jobs)
        {
          if (!job.IsComplete)
          {
            filtered.Add(job);
          }
        }
        return filtered;
      }
    }

    public virtual IBackgroundJob FindJob(Int32 jobNumber)
    {
      using (RWLock.AsReader(_lock))
      {
        foreach (IBackgroundJob job in _jobs)
        {
          if (job.JobNumber == jobNumber)
          {
            return job;
          }
        }
      }
      return null;
    }

    public virtual void AddJob(IBackgroundJob job)
    {
      if (job.JobNumber > 0)
      {
        throw new InvalidOperationException();
      }
      using (RWLock.AsReader(_lock))
      {
        if (_jobs.Contains(job))
        {
          throw new InvalidOperationException();
        }
        job.JobNumber = _jobs.Count + 1;
        _jobs.Add(job);
      }
    }

    public virtual void SaveJob(IBackgroundJob job)
    {
      if (job.JobNumber == 0)
      {
        AddJob(job);
      }
    }
    #endregion
  }
}
