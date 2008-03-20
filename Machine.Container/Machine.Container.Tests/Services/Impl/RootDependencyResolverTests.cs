using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class RootDependencyResolverTests : ScaffoldTests<RootDependencyResolver>
  {
    #region Member Data
    private readonly ServiceEntry _entry = ServiceEntryHelper.NewEntry();
    private IDependencyResolver _resolver1;
    private IDependencyResolver _resolver2;
    #endregion

    #region Test Methods
    /*
    [Test]
    public void CanResolveDependency_CantResolveAtAll_ThrowsAfterTrying()
    {
      Run(delegate
      {
        Expect.Call(_resolver1.CanResolveDependency(Get<ICreationServices>(), _entry)).Return(false);
        Expect.Call(_resolver2.CanResolveDependency(Get<ICreationServices>(), _entry)).Return(false);
      });
      Assert.IsFalse(_target.CanResolveDependency(Get<ICreationServices>(), _entry));
    }

    [Test]
    public void CanResolveDependency_GetsResolved_Returns()
    {
      Run(delegate
      {
        Expect.Call(_resolver1.CanResolveDependency(Get<ICreationServices>(), _entry)).Return(true);
      });
      Assert.IsTrue(_target.CanResolveDependency(Get<ICreationServices>(), _entry));
    }*/
    #endregion

    #region Methods
    protected override RootDependencyResolver Create()
    {
      _resolver1 = _mocks.DynamicMock<IDependencyResolver>();
      _resolver2 = _mocks.DynamicMock<IDependencyResolver>();
      return new RootDependencyResolver(new IDependencyResolver[] { _resolver1, _resolver2 });
    }
    #endregion
  }
}