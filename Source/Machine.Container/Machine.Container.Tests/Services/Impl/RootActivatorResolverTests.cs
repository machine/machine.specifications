using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class RootActivatorResolverTests : ScaffoldTests<RootActivatorResolver>
  {
    #region Member Data
    private readonly ServiceEntry _entry = ServiceEntryHelper.NewEntry();
    private IActivatorResolver _resolver1;
    private IActivatorResolver _resolver2;
    #endregion

    #region Test Methods
    [Test]
    public void ResolveActivator_CantResolveAtAll_ThrowsAfterTrying()
    {
      Run(delegate
      {
        Expect.Call(_resolver1.ResolveActivator(Get<ICreationServices>(), _entry)).Return(null);
        Expect.Call(_resolver2.ResolveActivator(Get<ICreationServices>(), _entry)).Return(null);
      });
      Assert.IsNull(_target.ResolveActivator(Get<ICreationServices>(), _entry));
    }

    [Test]
    public void CanResolveDependency_GetsResolved_Returns()
    {
      Run(delegate
      {
        Expect.Call(_resolver1.ResolveActivator(Get<ICreationServices>(), _entry)).Return(Get<IActivator>());
      });
      Assert.AreEqual(Get<IActivator>(), _target.ResolveActivator(Get<ICreationServices>(), _entry));
    }
    #endregion

    #region Methods
    protected override RootActivatorResolver Create()
    {
      _resolver1 = _mocks.DynamicMock<IActivatorResolver>();
      _resolver2 = _mocks.DynamicMock<IActivatorResolver>();
      return new RootActivatorResolver(new IActivatorResolver[] { _resolver1, _resolver2 });
    }
    #endregion
  }
}