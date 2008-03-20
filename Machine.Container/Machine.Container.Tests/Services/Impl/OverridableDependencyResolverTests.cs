using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class OverridableDependencyResolverTests : ScaffoldTests<OverridableDependencyResolver>
  {
    #region Member Data
    private readonly ServiceEntry _entry = ServiceEntryHelper.NewEntry();
    #endregion

    #region Test Methods
    [Test]
    public void ResolveDependency_Always_ChecksOverrideLookupService()
    {
      object instance = new object();
      Run(delegate
      {
        SetupResult.For(Get<ICreationServices>().Overrides).Return(Get<IOverrideLookup>());
        Expect.Call(Get<IOverrideLookup>().LookupOverride(_entry)).Return(instance);
      });
      // Assert.AreEqual(instance, _target.ResolveDependency(Get<ICreationServices>(), _entry));
    }
    #endregion
  }
}