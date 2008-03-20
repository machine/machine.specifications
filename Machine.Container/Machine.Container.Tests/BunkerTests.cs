using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Container.AutoMocking;
using Machine.Container.Model;

using NUnit.Framework;

using Rhino.Mocks;

namespace Machine.Container
{
  public class BunkerTests
  {
    #region Member Data
    protected MockRepository _mocks;
    protected AutoMockingContainer _container;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public virtual void Setup()
    {
      _mocks = new MockRepository();
      _container = new AutoMockingContainer(_mocks);
      _container.Initialize();
    }
    #endregion

    #region Methods
    public T Create<T>()
    {
      return _container.New<T>();
    }

    public T Get<T>() where T : class
    {
      return _container.Get<T>();
    }
    #endregion

    #region Methods
    protected static ConstructorCandidate CreateCandidate(Type type, params Type[] parameterTypes)
    {
      ConstructorInfo ctor = type.GetConstructor(parameterTypes);
      return new ConstructorCandidate(ctor);
    }
    #endregion
  }
}
