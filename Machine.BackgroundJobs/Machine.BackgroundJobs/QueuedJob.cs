using System;
using Machine.Core.Services;
using Machine.BackgroundJobs.Services;

namespace Machine.BackgroundJobs
{
  public class QueuedJob : IRunnable
  {
    #region Member Data
    private readonly IBackgroundJob _job;
    private readonly IBackgroundJobHandler _handler;
    #endregion

    #region Properties
    public IBackgroundJob Job
    {
      get { return _job; }
    }

    public IBackgroundJobHandler Handler
    {
      get { return _handler; }
    }
    #endregion

    #region QueuedJob()
    public QueuedJob(IBackgroundJob job, IBackgroundJobHandler handler)
    {
      if (job == null) throw new ArgumentNullException("job");
      if (handler == null) throw new ArgumentNullException("handler");
      _job = job;
      _handler = handler;
    }
    #endregion

    #region IRunnable Members
    public void Run()
    {
      try
      {
        _job.IsStarted = true;
        _handler.HandleJob(_job);
      }
      catch (Exception error)
      {
        _job.Error = error;
      }
      finally
      {
        _job.IsComplete = true;
      }
    }
    #endregion
  }
}