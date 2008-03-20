using System;
using System.Collections.Generic;

using Machine.Container.Services;
using Machine.Container.Services.Impl;

using Rhino.Mocks;

namespace Machine.Container.AutoMocking
{
  public class AutoMockingContainer : BunkerContainer
  {
    private readonly MockRepository _mocks;
    private MockingDependencyResolver _mockingDependencyResolver;

    public AutoMockingContainer(MockRepository mocks)
    {
      _mocks = mocks;
    }

    public override IDependencyResolver CreateDependencyResolver()
    {
      _mockingDependencyResolver = new MockingDependencyResolver(_mocks);
      return new RootDependencyResolver(new OverridableDependencyResolver(), new ActivatorLookupDependencyResolver(), _mockingDependencyResolver, new ThrowingDependencyResolver());
    }

    public virtual TService Get<TService>()
    {
      return _mockingDependencyResolver.Get<TService>();
    }
  }
}
