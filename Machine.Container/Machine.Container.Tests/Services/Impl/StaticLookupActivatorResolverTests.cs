using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class StaticLookupActivatorResolverTests : ScaffoldTests<StaticLookupActivatorResolver>
  {
    #region Member Data
    private readonly ServiceEntry _entry = ServiceEntryHelper.NewEntry();
    #endregion

    #region Test Methods
    [Test]
    public void ResolveActivator_IsThere_ReturnsStaticActivator()
    {
      object instance = new object();
      Run(delegate
      {
        SetupResult.For(Get<ICreationServices>().Overrides).Return(Get<IOverrideLookup>());
        SetupResult.For(Get<IOverrideLookup>().LookupOverride(_entry)).Return(instance);
        SetupResult.For(Get<ICreationServices>().ActivatorStrategy).Return(Get<IActivatorStrategy>());
        SetupResult.For(Get<IActivatorStrategy>().CreateStaticActivator(_entry, instance)).Return(Get<IActivator>());
      });
      Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(Get<ICreationServices>(), _entry));
    }

    [Test]
    public void ResolveActivator_NotThere_ReturnsNull()
    {
      Run(delegate
      {
        SetupResult.For(Get<ICreationServices>().Overrides).Return(Get<IOverrideLookup>());
        SetupResult.For(Get<IOverrideLookup>().LookupOverride(_entry)).Return(null);
      });
      Assert.IsNull(_target.ResolveActivator(Get<ICreationServices>(), _entry));
    }
    #endregion
  }
}