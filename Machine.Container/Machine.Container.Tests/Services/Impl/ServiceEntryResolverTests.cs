using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ServiceEntryResolverTests : ScaffoldTests<ServiceEntryResolver>
  {
    #region Member Data
    private IServiceGraph _serviceGraph;
    private IServiceDependencyInspector _serviceDependencyInspector;
    private IServiceEntryFactory _serviceEntryFactory;
    private ICreationServices _creationServices;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _serviceGraph = Get<IServiceGraph>();
      _serviceDependencyInspector = Get<IServiceDependencyInspector>();
      _serviceEntryFactory = Get<IServiceEntryFactory>();
      _creationServices = new CreationServices(Get<IActivatorStore>(), Get<ILifestyleStore>(), Get<IActivatorStrategy>(), Get<IOverrideLookup>());
    }
    #endregion

    #region Test Methods
    [Test]
    public void CreateEntryIfMissing_NotThereYet_JustCreatesEntryNothingElse()
    {
      ServiceEntry entry = new ServiceEntry(typeof(Service1), typeof(Service1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.Lookup(typeof(Service1))).Return(null);
        Expect.Call(_serviceEntryFactory.CreateServiceEntry(typeof(Service1), typeof(Service1), LifestyleType.Singleton)).Return(entry);
        _serviceGraph.Add(entry);
      }
      Assert.AreEqual(entry, _target.CreateEntryIfMissing(typeof(Service1)));
      _mocks.VerifyAll();
    }

    [Test]
    public void CreateEntryIfMissing_IsThere_JustReturnsTheOldOne()
    {
      ServiceEntry entry = new ServiceEntry(typeof(Service1), typeof(Service1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.Lookup(typeof(Service1))).Return(entry);
      }
      Assert.AreEqual(entry, _target.CreateEntryIfMissing(typeof(Service1)));
      _mocks.VerifyAll();
    }

    [Test]
    [ExpectedException(typeof(ServiceResolutionException))]
    public void CreateEntryIfMissing_IsThereWithOtherImplementation_Throws()
    {
      ServiceEntry entry = new ServiceEntry(typeof(IService1), typeof(Service1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.Lookup(typeof(IService1))).Return(entry);
      }
      _target.CreateEntryIfMissing(typeof(IService1), typeof(Service1DependsOn2));
      _mocks.VerifyAll();
    }
    /*
    [Test]
    public void ResolveEntry_NotInGraph_Throws()
    {
      ServiceEntry entry = new ServiceEntry(typeof(IService1), typeof(IService1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.LookupLazily(typeof(IService1))).Return(null);
        Expect.Call(_serviceEntryFactory.CreateServiceEntry(typeof(IService1), typeof(IService1), LifestyleType.Singleton)).Return(entry);
        Expect.Call(Get<IDependencyResolver>().ResolveDependency(_creationServices, entry)).Return(null);
      }
      Assert.IsNull(_target.ResolveEntry(_creationServices, typeof(IService1)));
      _mocks.VerifyAll();
    }

    [Test]
    public void ResolveEntry_InGraphNoDependencies_SetsResolveAndReturns()
    {
      ServiceEntry entry = new ServiceEntry(typeof(IService1), typeof(Service1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.LookupLazily(typeof(IService1))).Return(entry);
        Expect.Call(_serviceDependencyInspector.SelectConstructor(typeof (Service1))).Return(new ConstructorCandidate(null));
      }
      Assert.AreEqual(entry, _target.ResolveEntry(_creationServices, typeof(IService1)));
      Assert.IsTrue(entry.AreDependenciesResolved);
      _mocks.VerifyAll();
    }

    [Test]
    public void ResolveEntry_InResolved_DoesNotRecurse()
    {
      ServiceEntry entry = new ServiceEntry(typeof(IService1), typeof(Service1), LifestyleType.Singleton);
      using (_mocks.Record())
      {
        Expect.Call(_serviceGraph.LookupLazily(typeof(IService1))).Return(entry);
      }
      entry.AreDependenciesResolved = true;
      Assert.AreEqual(entry, _target.ResolveEntry(_creationServices, typeof(IService1)));
      _mocks.VerifyAll();
    }*/
    #endregion
  }
}
