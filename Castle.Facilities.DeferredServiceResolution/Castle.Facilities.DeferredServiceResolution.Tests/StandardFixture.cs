using System;
using System.Collections.Generic;

using Castle.MicroKernel;

using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Facilities.DeferredServiceResolution
{
  public abstract class StandardFixture<TClass>
  {
    protected TClass _target;
    protected MockRepository _mocks;
    protected IKernel _kernel;
    protected IDependencyResolver _dependencyResolver;

    [SetUp]
    public virtual void Setup()
    {
      _mocks = new MockRepository();
      _kernel = _mocks.DynamicMock<IKernel>();
      _dependencyResolver = _mocks.DynamicMock<IDependencyResolver>();
      _target = Create();
    }

    public abstract TClass Create();
  }
}