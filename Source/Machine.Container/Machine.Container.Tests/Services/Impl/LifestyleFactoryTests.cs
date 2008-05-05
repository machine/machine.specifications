using System;
using System.Collections.Generic;

using Machine.Container.Lifestyles;
using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class LifestyleFactoryTests : ScaffoldTests<LifestyleFactory>
  {
    #region Test Methods
    [Test]
    public void CreateSingletonLifestyle_Always_ReturnsNewLifestyle()
    {
      Assert.IsInstanceOfType(typeof(SingletonLifestyle), _target.CreateSingletonLifestyle(ServiceEntryHelper.NewEntry()));
    }

    [Test]
    public void CreateTransientLifestyle_Always_ReturnsNewLifestyle()
    {
      Assert.IsInstanceOfType(typeof(TransientLifestyle), _target.CreateTransientLifestyle(ServiceEntryHelper.NewEntry()));
    }

    [Test]
    public void CreateLifestyle_Singleton_ReturnsNewLifestyle()
    {
      Assert.IsInstanceOfType(typeof(SingletonLifestyle), _target.CreateLifestyle(ServiceEntryHelper.NewEntry()));
    }

    [Test]
    public void CreateLifestyle_Transient_ReturnsNewLifestyle()
    {
      Assert.IsInstanceOfType(typeof(TransientLifestyle), _target.CreateLifestyle(ServiceEntryHelper.NewEntry(LifestyleType.Transient)));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateLifestyle_UnknownType_Throws()
    {
      _target.CreateLifestyle(ServiceEntryHelper.NewEntry((LifestyleType)343));
    }
    #endregion
  }
}