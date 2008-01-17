using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Sample;
using Machine.Core;

using Castle.Windsor;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class WindsorJobServicesLocatorTests : StandardFixture<WindsorJobServicesLocator>
  {
    public override WindsorJobServicesLocator Create()
    {
      return new WindsorJobServicesLocator(Get<IWindsorContainer>());
    }

    [Test]
    public void LocateJobHandler_ValidHandler_ReturnsTheHandler()
    {
      using (_mocks.Record())
      {
        SetupResult.For(Get<IWindsorContainer>().Resolve(typeof(LongRunningJobHandler))).Return(Get<IBackgroundJobHandler>());
      }
      Assert.AreEqual(Get<IBackgroundJobHandler>(), _target.LocateJobHandler(new LongRunningJob()));
    }

    [Test]
    public void LocateJobRepository_ValidHandler_ReturnsTheRepository()
    {
      using (_mocks.Record())
      {
        SetupResult.For(Get<IWindsorContainer>().Resolve(typeof(LongRunningJobRepository))).Return(Get<IJobRepository>());
      }
      Assert.AreEqual(Get<IJobRepository>(), _target.LocateRepository(new LongRunningJob()));
    }
  }
}