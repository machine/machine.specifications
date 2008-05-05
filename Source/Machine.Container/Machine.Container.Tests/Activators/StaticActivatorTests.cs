using System;
using System.Collections.Generic;

using Machine.Container.Services;
using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Activators
{
  [TestFixture]
  public class StaticActivatorTests : MachineContainerTestsFixture
  {
    #region Member Data
    private StaticActivator _activator;
    private ServiceEntry _entry;
    private object _instance;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _entry = ServiceEntryHelper.NewEntry();
      _instance = new object();
      _activator = new StaticActivator(_entry, _instance);
    }
    #endregion

    #region Test Methods
    [Test]
    public void Create_Always_ReturnsInstance()
    {
      Assert.AreEqual(_instance, _activator.Activate(Get<ICreationServices>()));
      Assert.AreEqual(_instance, _activator.Activate(Get<ICreationServices>()));
    }
    #endregion
  }
}