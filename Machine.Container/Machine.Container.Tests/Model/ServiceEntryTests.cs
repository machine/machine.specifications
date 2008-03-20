using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

using Machine.Container.Services.Impl;

namespace Machine.Container.Model
{
  [TestFixture]
  public class ServiceEntryTests : ScaffoldTests<ServiceEntry>
  {
    #region Test Methods
    [Test]
    public void ToString_Always_ReturnsAString()
    {
      Run();
      Assert.IsNotNull(_target.ToString());
    }
    #endregion

    #region Methods
    protected override ServiceEntry Create()
    {
      return new ServiceEntry(typeof(IService1), typeof(SimpleService1), LifestyleType.Singleton);
    }
    #endregion
  }
}