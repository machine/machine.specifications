using System;
using System.Collections.Generic;

using Machine.BackgroundJobs.Sample;
using Machine.Core;

using NUnit.Framework;

namespace Machine.BackgroundJobs.Services.Impl
{
  [TestFixture]
  public class SimpleJobRepositoryLocatorTests : StandardFixture<SimpleJobRepositoryLocator>
  {
    public override SimpleJobRepositoryLocator Create()
    {
      return new SimpleJobRepositoryLocator();
    }

    [Test]
    public void LocateJobRepository_FirstCall_IsRepository()
    {
      using (_mocks.Record())
      {
      }
      Assert.IsNotNull(_target.LocateJobRepository(typeof(LongRunningJob)));
      _mocks.VerifyAll();
    }

    [Test]
    public void LocateJobRepository_SecondCall_IsSameRepository()
    {
      using (_mocks.Record())
      {
      }
      IJobRepository repository = _target.LocateJobRepository(typeof(LongRunningJob));
      Assert.AreEqual(repository, _target.LocateJobRepository(typeof(LongRunningJob)));
      _mocks.VerifyAll();
    }
  }
}