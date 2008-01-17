using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Core.Services;
using Machine.BackgroundJobs.Sample;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class BackgroundJobSpawnerTests : StandardFixture<BackgroundJobSpawner>
  {
    public override BackgroundJobSpawner Create()
    {
      return new BackgroundJobSpawner(Get<IThreadManager>(), Get<IJobHandlerLocator>());
    }

    [Test]
    public void Spawn_Always_LocatesHandlerAndCreatesThreadWithQeuedJob()
    {
      LongRunningJob job = new LongRunningJob();
      job.JobNumber = 1;
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobHandlerLocator>().LocateJobHandler(typeof(LongRunningJob))).Return(Get<IBackgroundJobHandler>());
        Expect.Call(Get<IThreadManager>().CreateThread(Get<IRunnable>())).Constraints(Is.TypeOf(typeof(QueuedJob))).Return(Get<IThread>());
        Get<IThread>().Start();
      }
      _target.SpawnJob(job);
      _mocks.VerifyAll();
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void Spawn_NotSavedNoNumber_Throws()
    {
      LongRunningJob job = new LongRunningJob();
      using (_mocks.Record())
      {
      }
      _target.SpawnJob(job);
    }
  }
}