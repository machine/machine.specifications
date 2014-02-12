using System;
using System.Collections.Generic;

using Machine.Container;
using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

using Rhino.Mocks;

namespace Machine.Testing.AutoMocking
{
  [ObsoleteEx(Message = "Use FluentAssertions or Mocking library of choice directly", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8")]
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

  [ObsoleteEx(Message = "Use FluentAssertions or Mocking library of choice directly", RemoveInVersion = "0.9", TreatAsErrorFromVersion = "0.8")]
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