using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

namespace ObjectFactories
{
  public abstract class ObjectFactoriesTests<TTarget>
  {
    protected TTarget _target;
    protected NewRoot _new;
    protected MockRepository _mocks;
    protected MockAssemblyFactory _mockAssemblyFactory;

    public NewRoot New
    {
      get { return _new; }
    }

    public MockAssemblyFactory MockAssemblyFactory
    {
      get { return _mockAssemblyFactory; }
    }

    [SetUp]
    public virtual void Setup()
    {
      _mocks = new MockRepository();
      _mockAssemblyFactory = new MockAssemblyFactory(_mocks);
      _new = new NewRoot(_mocks);
      _target = Create();
    }

    public abstract TTarget Create();
  }
}
