using System;
using System.Collections.Generic;

using Machine.Container.Activators;
using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Activators
{
  [TestFixture]
  public class DefaultActivatorTests : ScaffoldTests<DefaultActivator>
  {
    #region Member Data
    private ServiceEntry _entry;
    private object _instance;
    private object _parameter1;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      _entry = ServiceEntryHelper.NewEntry();
      _instance = new object();
      _parameter1 = new object();
      base.Setup();
    }
    #endregion

    #region Test Methods
    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Create_NotResolved_Throws()
    {
      using (_mocks.Record())
      {
      }
      using (_mocks.Playback())
      {
        _target.Create(Get<ICreationServices>());
      }
    }

    [Test]
    public void Create_NoDependencies_CallsConstructor()
    {
      using (_mocks.Record())
      {
        SetupMocks();
        _entry.AreDependenciesResolved = true;
        Expect.Call(Get<IObjectFactory>().CreateObject(_entry.ConstructorCandidate, new object[0])).Return(_instance);
      }
      using (_mocks.Playback())
      {
        object instance = _target.Create(Get<ICreationServices>());
        Assert.AreEqual(_instance, instance);
      }
    }

    [Test]
    public void Create_OneDependency_CallsConstructor()
    {
      ServiceEntry dependency = ServiceEntryHelper.NewEntry();
      dependency.Activator = Get<IActivator>();
      using (_mocks.Record())
      {
        SetupMocks(dependency);
        Expect.Call(Get<IActivator>().Create(Get<ICreationServices>())).Return(_parameter1);
        Expect.Call(Get<IObjectFactory>().CreateObject(_entry.ConstructorCandidate, new object[] { _parameter1 })).Return(_instance);
      }
      using (_mocks.Playback())
      {
        object instance = _target.Create(Get<ICreationServices>());
        Assert.AreEqual(_instance, instance);
      }
    }
    #endregion

    #region Methods
    protected virtual void SetupMocks(params ServiceEntry[] dependencies)
    {
      SetupResult.For(Get<ICreationServices>().Progress).Return(new Stack<ServiceEntry>());
      SetupResult.For(Get<ICreationServices>().ActivatorStore).Return(Get<IActivatorStore>());
      _entry.ConstructorCandidate = CreateCandidate(typeof(ExampleNoArguments));
      _entry.Dependencies.AddRange(dependencies);
    }

    protected override DefaultActivator Create()
    {
      return new DefaultActivator(Get<IObjectFactory>(), Get<IServiceDependencyInspector>(), Get<IServiceEntryResolver>(), _entry);
    }
    #endregion
  }
}