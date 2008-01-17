using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Sample;
using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class BackgroundJobQueuerTests : StandardFixture<BackgroundJobQueuer>
  {
    public override BackgroundJobQueuer Create()
    {
      return new BackgroundJobQueuer(Get<IJobServicesLocator>());
    }

    [Test]
    public void QueueJob_Always_LocatesRepositoryAndSaves()
    {
      LongRunningJob job = new LongRunningJob();
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobServicesLocator>().LocateRepository(typeof(LongRunningJob))).Return(Get<IJobRepository>());
        Get<IJobRepository>().SaveJob(job);
      }
      Assert.AreEqual(job, _target.QueueJob(job));
      _mocks.VerifyAll();
    }
  }
}