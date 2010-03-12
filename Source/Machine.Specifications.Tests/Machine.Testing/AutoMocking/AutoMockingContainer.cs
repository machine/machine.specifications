using System;
using System.Collections.Generic;

using Machine.Container;
using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

using Rhino.Mocks;

namespace Machine.Testing.AutoMocking
{
  public class AutoMockingContainer : MachineContainer
  {
    private readonly MockingDependencyResolver _mockingDependencyResolver;

    public AutoMockingContainer(MockRepository mocks)
      : this(new MockingDependencyResolver(mocks))
    {
    }

    public AutoMockingContainer(MockingDependencyResolver mockingDependencyResolver)
      : base(new CompartmentalizedMachineContainer(new MockingDependencyResolverFactory(mockingDependencyResolver)))
    {
      _mockingDependencyResolver = mockingDependencyResolver;
    }

    public virtual TService Get<TService>()
    {
      return _mockingDependencyResolver.Get<TService>();
    }
  }
  public class MockingDependencyResolverFactory : DefaultContainerInfrastructureFactory
  {
    private readonly MockingDependencyResolver _mockingDependencyResolver;

    public MockingDependencyResolverFactory(MockingDependencyResolver mockingDependencyResolver)
    {
      _mockingDependencyResolver = mockingDependencyResolver;
    }

    #region IContainerInfrastructureFactory Members
    public override IRootActivatorResolver CreateDependencyResolver()
    {
      IRootActivatorResolver resolver = base.CreateDependencyResolver();
      resolver.AddAfter(typeof(ActivatorStoreActivatorResolver), _mockingDependencyResolver);
      return resolver;
    }
    #endregion
  }
}