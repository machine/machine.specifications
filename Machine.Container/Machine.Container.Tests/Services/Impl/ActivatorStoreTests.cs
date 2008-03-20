using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ActivatorStoreTests : BunkerTests
  {
    #region Member Data
    private ActivatorStore _store;
    private ServiceEntry _entry;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _entry = ServiceEntryHelper.NewEntry();
      _store = Create<ActivatorStore>();
    }
    #endregion

    #region Test Methods
    [Test]
    public void ResolveActivator_First_AsksStrategy()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<ILifestyleFactory>().CreateLifestyle(_entry)).Return(Get<ILifestyle>());
        Expect.Call(Get<IActivatorStrategy>().CreateLifestyleActivator(Get<ILifestyle>())).Return(Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _store.ResolveActivator(_entry));
      }
    }

    [Test]
    public void ResolveActivator_Second_ReturnsFirst()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<ILifestyleFactory>().CreateLifestyle(_entry)).Return(Get<ILifestyle>());
        Expect.Call(Get<IActivatorStrategy>().CreateLifestyleActivator(Get<ILifestyle>())).Return(Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _store.ResolveActivator(_entry));
        Assert.AreEqual(Get<IActivator>(), _store.ResolveActivator(_entry));
      }
    }

    [Test]
    public void AddActivator_Always_CachesThatOne()
    {
      using (_mocks.Record())
      {
        _store.AddActivator(_entry, Get<IActivator>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<IActivator>(), _store.ResolveActivator(_entry));
      }
    }
    #endregion
  }
}
