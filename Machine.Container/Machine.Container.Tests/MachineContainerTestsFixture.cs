using System;
using System.Collections.Generic;
using System.Reflection;

using log4net.Appender;
using Machine.Container.AutoMocking;
using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container
{
  public class MachineContainerTestsFixture
  {
    #region Member Data
    protected static bool _loggingInitialized;
    protected MockRepository _mocks;
    protected AutoMockingContainer _container;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public virtual void Setup()
    {
      if (!_loggingInitialized)
      {
        OutputDebugStringAppender appender = new OutputDebugStringAppender();
        appender.Layout = new log4net.Layout.PatternLayout("%-5p %c{1} %m");
        log4net.Config.BasicConfigurator.Configure(appender);
        _loggingInitialized = true;
      }
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
      ConstructorCandidate constructorCandidate = new ConstructorCandidate(ctor);
      foreach (Type parameterType in parameterTypes)
      {
        constructorCandidate.Dependencies.Add(new ServiceDependency(parameterType, DependencyType.Constructor));
      }
      return constructorCandidate;
    }
    #endregion
  }
}
