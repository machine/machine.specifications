using System;
using Machine.BackgroundJobs.Services;
using Machine.BackgroundJobs.Services.Impl;
using Machine.Core.Services;

namespace Machine.BackgroundJobs.Sample
{
  public class LongRunningJobHandler : AbstractBackgroundJobHandler<LongRunningJob>
  {
    #region Member Data
    private readonly IThreadManager _threadManager;
    private readonly IJobRepository _jobRepository;
    #endregion

    #region LongRunningJobHandler()
    public LongRunningJobHandler(IThreadManager threadManager, IJobRepository jobRepository)
    {
      _threadManager = threadManager;
      _jobRepository = jobRepository;
    }
    #endregion

    #region IBackgroundJobHandler Members
    public override void HandleJob(LongRunningJob job)
    {
      DateTime started = DateTime.Now;
      while (true)
      {
        if (DateTime.Now - started > job.RunFor)
        {
          break;
        }
        _threadManager.Sleep(TimeSpan.FromSeconds(1.0));
        _jobRepository.SaveJob(job);
      }
    }
    #endregion
  }
}