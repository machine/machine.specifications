using System;
using System.Collections.Generic;

using Machine.Container.Activators;
using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class DefaultActivatorStrategyTests : BunkerTests
  {
    #region Member Data
    private IObjectFactory _objectFactory;
    private IActivatorResolver _activatorResolver;
    private IServiceDependencyInspector _serviceDependencyInspector;
    private IServiceEntryResolver _serviceEntryResolver;
    private DefaultActivatorStrategy _strategy;
    private ServiceEntry _entry;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _entry = ServiceEntryHelper.NewEntry();
      _objectFactory = _mocks.CreateMock<IObjectFactory>();
      _activatorResolver = _mocks.DynamicMock<IActivatorResolver>();
      _serviceDependencyInspector = _mocks.DynamicMock<IServiceDependencyInspector>();
      _serviceEntryResolver = _mocks.DynamicMock<IServiceEntryResolver>();
      _strategy = new DefaultActivatorStrategy(_objectFactory, _serviceEntryResolver, _serviceDependencyInspector);
    }
    #endregion

    #region Test Methods
    [Test]
    public void CreateLifestyleActivator_Always_CreatesDefaultActivator()
    {
      Assert.IsInstanceOfType(typeof(LifestyleActivator), _strategy.CreateLifestyleActivator(Get<ILifestyle>()));
    }

    [Test]
    public void CreateActivatorInstance_ReturnsInstanceActivator_ReturnsSameOne()
    {
      Assert.IsInstanceOfType(typeof(StaticActivator), _strategy.CreateStaticActivator(_entry, new object()));
    }

    [Test]
    public void CreateDefaultActivator_Always_IsDefaultActivator()
    {
      Assert.IsInstanceOfType(typeof(DefaultActivator), _strategy.CreateDefaultActivator(_entry));
    }
    #endregion
  }
}