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
    public void Create_Always_DefersToLifestyle()
    {
      _instance = new object();
      Run(delegate
      {
        Expect.Call(Get<ILifestyle>().Create(Get<ICreationServices>())).Return(_instance);
      });
      object instance = _target.Create(Get<ICreationServices>());
      Assert.AreEqual(_instance, instance);
    }
    #endregion
  }
}