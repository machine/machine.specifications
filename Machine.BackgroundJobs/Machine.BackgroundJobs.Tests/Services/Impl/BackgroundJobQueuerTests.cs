using System;
using System.Collections.Generic;

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
      using (_mocks.Record())
      {
        SetupResult.For(Get<IJobServicesLocator>().LocateRepository(Get<IBackgroundJob>())).Return(Get<IJobRepository>());
        Get<IJobRepository>().SaveJob(Get<IBackgroundJob>());
      }
      Assert.AreEqual(Get<IBackgroundJob>(), _target.QueueJob(Get<IBackgroundJob>()));
      _mocks.VerifyAll();
    }
  }
}