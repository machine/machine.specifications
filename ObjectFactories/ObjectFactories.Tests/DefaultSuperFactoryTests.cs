using System;
using System.Collections.Generic;

using NUnit.Framework;
using ObjectFactories.Services;
using ObjectFactories.Services.Impl;

namespace ObjectFactories
{
  [TestFixture]
  [Ignore]
  public class DefaultSuperFactoryTests : ObjectFactoriesTests<DefaultSuperFactory>
  {
    public override DefaultSuperFactory Create()
    {
      return new DefaultSuperFactory();
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateFactory_BadFactoryType_Throws()
    {
      _target.CreateFactory<User>();
    }

    [Test]
    public void CreateFactory_WithFactoryType_CreatesNewFactory()
    {
      IObjectFactory<User> factory1 = _target.CreateFactory<User>();
      Assert.IsNotNull(factory1);
      IObjectFactory<User> factory2 = _target.CreateFactory<User>();
      Assert.IsNotNull(factory2);
      Assert.AreNotEqual(factory1, factory2);
    }
  }
  public class User
  {
  }
  public class UserFactory : IObjectFactory<User>
  {
    #region IObjectFactory<User> Members
    public User Create()
    {
      return new User();
    }
    #endregion
  }
}
