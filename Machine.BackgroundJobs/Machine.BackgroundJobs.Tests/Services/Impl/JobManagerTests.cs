using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Sample;
using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class JobManagerTests : StandardFixture<JobManager>
  {
    private readonly List<IBackgroundJob> _jobs = new List<IBackgroundJob>();

    public override JobManager Create()
    {
      GetNormal<IJobRepository>();
      return new JobManager(Get<IJobRepositoryLocator>(), Get<IBackgroundJobQueuer>(), Get<IBackgroundJobSpawner>());
    }

    [Test]
    public void FindActiveJobs_Always_GetsFromRepository()
    {
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobRepositoryLocator>().LocateJobRepository(typeof(LongRunningJob))).Return(Get<IJobRepository>());
        SetupResult.For(Get<IJobRepository>().FindActiveJobs()).Return(_jobs);
      }
      Assert.AreEqual(_jobs, _target.FindActiveJobs(typeof(LongRunningJob)));
    }

    [Test]
    public void FindCompletedJobs_Always_GetsFromRepository()
    {
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobRepositoryLocator>().LocateJobRepository(typeof(LongRunningJob))).Return(Get<IJobRepository>());
        SetupResult.For(Get<IJobRepository>().FindCompletedJobs()).Return(_jobs);
      }
      Assert.AreEqual(_jobs, _target.FindCompletedJobs(typeof(LongRunningJob)));
    }

    [Test]
    public void FindJobs_Always_GetsFromRepository()
    {
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobRepositoryLocator>().LocateJobRepository(typeof(LongRunningJob))).Return(Get<IJobRepository>());
        SetupResult.For(Get<IJobRepository>().FindJobs()).Return(_jobs);
      }
      Assert.AreEqual(_jobs, _target.FindJobs(typeof(LongRunningJob)));
    }

    [Test]
    public void FindJob_Always_AsksRepository()
    {
      LongRunningJob job = new LongRunningJob();
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobRepositoryLocator>().LocateJobRepository(typeof(LongRunningJob))).Return(Get<IJobRepository>());
        SetupResult.For(Get<IJobRepository>().FindJob(1)).Return(job);
      }
      Assert.AreEqual(job, _target.FindJob(typeof(LongRunningJob), 1));
    }

    [Test]
    public void QueueJob_Always_Queues()
    {
      LongRunningJob job = new LongRunningJob();
      using (_mocks.Record())
      {
        Expect.Call(Get<IBackgroundJobQueuer>().QueueJob(job)).Return(job);
      }
      _target.QueueJob(job);
      _mocks.VerifyAll();
    }

    [Test]
    public void StartJob_Always_Spawns()
    {
      LongRunningJob job = new LongRunningJob();
      using (_mocks.Record())
      {
        Get<IBackgroundJobSpawner>().SpawnJob(job);
      }
      _target.StartJob(job);
      _mocks.VerifyAll();
    }
  }
}