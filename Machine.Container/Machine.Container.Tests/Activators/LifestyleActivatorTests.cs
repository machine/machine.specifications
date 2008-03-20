using System;
using System.Collections.Generic;

using Machine.Container.Services;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Activators
{
  [TestFixture]
  public class LifestyleActivatorTests : ScaffoldTests<LifestyleActivator>
  {
    #region Member Data
    private object _instance;
    #endregion

    #region Test Methods
    [Test]
    public void CanActivate_Always_DefersToLifestyle()
    {
      _instance = new object();
      Run(delegate
      {
        Expect.Call(Get<ILifestyle>().CanActivate(Get<ICreationServices>())).Return(true);
      });
      Assert.IsTrue(_target.CanActivate(Get<ICreationServices>()));
    }

    [Test]
    public void Create_Always_DefersToLifestyle()
    {
      _instance = new object();
      Run(delegate
      {
        Expect.Call(Get<ILifestyle>().Activate(Get<ICreationServices>())).Return(_instance);
      });
      object instance = _target.Activate(Get<ICreationServices>());
      Assert.AreEqual(_instance, instance);
    }
    #endregion
  }
}