using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class LifestyleStoreTests : ScaffoldTests<LifestyleStore>
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
        Expect.Call(Get<ILifestyleFactory>().CreateLifestyle(_entry)).Return(Get<ILifestyle>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<ILifestyle>(), _target.ResolveLifestyle(_entry));
      }
    }

    [Test]
    public void ResolveActivator_Second_ReturnsFirst()
    {
      using (_mocks.Record())
      {
        Expect.Call(Get<ILifestyleFactory>().CreateLifestyle(_entry)).Return(Get<ILifestyle>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<ILifestyle>(), _target.ResolveLifestyle(_entry));
        Assert.AreEqual(Get<ILifestyle>(), _target.ResolveLifestyle(_entry));
      }
    }

    [Test]
    public void AddActivator_Always_CachesThatOne()
    {
      using (_mocks.Record())
      {
        _target.AddLifestyle(_entry, Get<ILifestyle>());
      }
      using (_mocks.Playback())
      {
        Assert.AreEqual(Get<ILifestyle>(), _target.ResolveLifestyle(_entry));
      }
    }
    #endregion
  }
}