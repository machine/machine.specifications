using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ActivatorStoreTests : ScaffoldTests<ActivatorStore>
  {
    #region Member Data
    private ServiceEntry _entry;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _entry = ServiceEntryHelper.NewEntry();
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ResolveActivator_DoesNotHaveOne_Throws()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<IActivatorStrategy>().CreateLifestyleActivator(Get<ILifestyle>())).Return(Get<IActivator>());
      }
      _target.ResolveActivator(_entry);
    }

    [Test]
    public void ResolveActivator_HasOne_ReturnsIt()
    {
      using (_mocks.Record())
      {
      }
      _target.AddActivator(_entry, Get<IActivator>());
      Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
    }

    [Test]
    public void AddActivator_Always_CachesThatOne()
    {
      using (_mocks.Record())
      {
        _target.AddActivator(_entry, Get<IActivator>());
      }
      Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
    }
    #endregion
  }
}
