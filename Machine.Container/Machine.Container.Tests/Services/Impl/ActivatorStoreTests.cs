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
    public void ResolveActivator_First_AsksStrategy()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<ILifestyleStore>().ResolveLifestyle(_entry)).Return(Get<ILifestyle>());
        Expect.Call(Get<IActivatorStrategy>().CreateLifestyleActivator(Get<ILifestyle>())).Return(Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
      }
    }

    [Test]
    public void ResolveActivator_Second_ReturnsFirst()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<ILifestyleStore>().ResolveLifestyle(_entry)).Return(Get<ILifestyle>());
        Expect.Call(Get<IActivatorStrategy>().CreateLifestyleActivator(Get<ILifestyle>())).Return(Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
        Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
      }
    }

    [Test]
    public void AddActivator_Always_CachesThatOne()
    {
      using (_mocks.Record())
      {
        _target.AddActivator(_entry, Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(_entry));
      }
    }
    #endregion
  }
}
