using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Services;
using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.BackgroundJobs
{
  [TestFixture]
  public class QueuedJobTests : StandardFixture<QueuedJob>
  {
    public override QueuedJob Create()
    {
      return new QueuedJob(GetNormal<IBackgroundJob>(), GetNormal<IBackgroundJobHandler>());
    }

    [Test]
    public void Run_Always_StartsAndCompletes()
    {
      using (_mocks.Ordered())
      {
        Get<IBackgroundJob>().IsStarted = true;
        Get<IBackgroundJob>().PercentageComplete = 0.0;
        Get<IBackgroundJobHandler>().HandleJob(Get<IBackgroundJob>());
        Get<IBackgroundJob>().IsComplete = true;
        Get<IBackgroundJob>().PercentageComplete = 100.0;
      }
      _mocks.ReplayAll();
      _target.Run();
      _mocks.VerifyAll();
    }

    [Test]
    public void Run_Exception_SwallowsButSavesInJob()
    {
      ArgumentException error = new ArgumentException();
      using (_mocks.Ordered())
      {
        Get<IBackgroundJob>().IsStarted = true;
        Get<IBackgroundJob>().PercentageComplete = 0.0;
        Get<IBackgroundJobHandler>().HandleJob(Get<IBackgroundJob>());
        LastCall.Throw(error);
        Get<IBackgroundJob>().Error = error;
        Get<IBackgroundJob>().IsComplete = true;
        Get<IBackgroundJob>().PercentageComplete = 100.0;
      }
      _mocks.ReplayAll();
      _target.Run();
      _mocks.VerifyAll();
    }
  }
}