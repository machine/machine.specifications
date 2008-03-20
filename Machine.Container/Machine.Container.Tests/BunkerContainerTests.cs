using System;
using System.Collections.Generic;
using Machine.Container.Model;
using Machine.Container.Services.Impl;

using NUnit.Framework;

namespace Machine.Container
{
  [TestFixture]
  public class BunkerContainerTests : BunkerTests
  {
    #region Member Data
    private BunkerContainer _bunker;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _bunker = new BunkerContainer();
      _bunker.Initialize();
    }
    #endregion

    #region Test Methods
    [Test]
    public void AddServiceNoInterface_NoDependencies_ResolvesEntry()
    {
      _bunker.AddService<IService1, Service1>(LifestyleType.Singleton);
      Assert.IsNotNull(_bunker.Resolve<IService1>());
    }

    [Test]
    public void AddServiceNoInterface_SingleDependency_ResolvesEntry()
    {
      _bunker.AddService<Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_bunker.Resolve<IService2>());
    }

    [Test]
    public void HasService_DoesNot_IsFalse()
    {
      Assert.IsFalse(_bunker.HasService<IService1>());
    }

    [Test]
    public void HasService_Does_IsTrue()
    {
      _bunker.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsTrue(_bunker.HasService<IService1>());
    }

    [Test]
    public void HasService_DoesButNotUnderInterface_IsTrue()
    {
      _bunker.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsTrue(_bunker.HasService<IService1>());
    }

    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void AddService_SingleDependencyNotThere_Throws()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.Resolve<IService2>();
    }

    [Test]
    public void AddService_SingleDependencyThere_ResolvesInstance()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_bunker.Resolve<IService2>());
    }

    [Test]
    public void AddService_LazilySingleDependencyThere_ResolvesInstance()
    {
      _bunker.AddService<Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_bunker.Resolve<IService2>());
    }

    [Test]
    [ExpectedException(typeof(AmbiguousServicesException))]
    public void AddService_LazilyMultipleDependencies_ThrowsAmbiguous()
    {
      _bunker.AddService<Service1DependsOn2>(LifestyleType.Singleton);
      _bunker.AddService<SimpleService1>(LifestyleType.Singleton);
      _bunker.Resolve<IService1>();
    }

    [Test]
    public void AddService_CircularDependency_Throws()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<IService1, Service1DependsOn2>(LifestyleType.Singleton);
    }

    [Test]
    [ExpectedException(typeof(ServiceResolutionException))]
    public void AddService_Duplicate_Throws()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<IService2, Service1DependsOn2>(LifestyleType.Singleton);
    }

    [Test]
    [ExpectedException(typeof(CircularDependencyException))]
    public void Resolve_CircularDependency_Throws()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<IService1, Service1DependsOn2>(LifestyleType.Singleton);
      _bunker.Resolve<IService2>();
    }

    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void Resolve_WaitingDependencies_Throws()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.Resolve<IService2>();
    }

    [Test]
    public void Resolve_NotWaitingDependencies_Works()
    {
      _bunker.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _bunker.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_bunker.Resolve<IService2>());
    }

    [Test]
    public void Resolve_Singleton_YieldsSameInstances()
    {
      _bunker.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      IService1 service1 = _bunker.Resolve<IService1>();
      IService1 service2 = _bunker.Resolve<IService1>();
      Assert.AreEqual(service1, service2);
    }

    [Test]
    public void Resolve_Transient_YieldsMultipleInstances()
    {
      _bunker.AddService<IService1, SimpleService1>(LifestyleType.Transient);
      IService1 service1 = _bunker.Resolve<IService1>();
      IService1 service2 = _bunker.Resolve<IService1>();
      Assert.AreNotEqual(service1, service2);
    }

    [Test]
    public void ResolveWithOverrides_WithOverrides_UsesOverride()
    {
      _bunker.AddService<IService1, Service1DependsOn2>(LifestyleType.Transient);
      Assert.IsNotNull(_bunker.ResolveWithOverrides<IService1>(new SimpleService2()));
    }
    #endregion
  }
  public class Service1 : IService1
  {
  }
  public class Service2DependsOn1 : IService2
  {
    private readonly IService1 _s1;
    public Service2DependsOn1(IService1 s1)
    {
      _s1 = s1;
    }
  }
  public class Service1DependsOn2 : IService1
  {
    private readonly IService2 _s2;
    public Service1DependsOn2(IService2 s2)
    {
      _s2 = s2;
    }
  }
  public class SimpleService1 : IService1
  {
  }
  public class SimpleService2 : IService2
  {
  }
}
