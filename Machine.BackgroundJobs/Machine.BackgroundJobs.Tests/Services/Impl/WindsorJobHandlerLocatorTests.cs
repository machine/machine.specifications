using System;
using System.Collections.Generic;

using Castle.Windsor;

using Machine.Core;
using Machine.BackgroundJobs.Sample;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class WindsorJobHandlerLocatorTests : StandardFixture<WindsorJobHandlerLocator>
  {
    public override WindsorJobHandlerLocator Create()
    {
      return new WindsorJobHandlerLocator(Get<IWindsorContainer>());
    }

    [Test]
    public void LocateJobHandler_Always_GetsFromContainer()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<IWindsorContainer>().Resolve(typeof(LongRunningJobHandler))).Return(Get<IBackgroundJobHandler>());
      }
      Assert.AreEqual(Get<IBackgroundJobHandler>(), _target.LocateJobHandler(typeof(LongRunningJob)));
      _mocks.VerifyAll();
    }
  }
}