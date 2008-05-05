using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services.Impl;

using NUnit.Framework;

namespace Machine.Container
{
  [TestFixture]
  public class MachineContainerTests : MachineContainerTestsFixture
  {
    #region Member Data
    private MachineContainer _machineContainer;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _machineContainer = new MachineContainer();
      _machineContainer.Initialize();
    }
    #endregion

    #region Test Methods
    [Test]
    public void AddServiceNoInterface_NoDependencies_ResolvesEntry()
    {
      _machineContainer.AddService<IService1, Service1>(LifestyleType.Singleton);
      Assert.IsNotNull(_machineContainer.Resolve<IService1>());
    }

    [Test]
    public void AddServiceNoInterface_SingleDependency_ResolvesEntry()
    {
      _machineContainer.AddService<Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_machineContainer.Resolve<IService2>());
    }

    [Test]
    public void HasService_DoesNot_IsFalse()
    {
      Assert.IsFalse(_machineContainer.HasService<IService1>());
    }

    [Test]
    public void HasService_Does_IsTrue()
    {
      _machineContainer.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsTrue(_machineContainer.HasService<IService1>());
    }

    [Test]
    public void HasService_DoesButNotUnderInterface_IsTrue()
    {
      _machineContainer.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsTrue(_machineContainer.HasService<IService1>());
    }

    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void AddService_SingleDependencyNotThere_Throws()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.Resolve<IService2>();
    }

    [Test]
    public void AddService_SingleDependencyThere_ResolvesInstance()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_machineContainer.Resolve<IService2>());
    }

    [Test]
    public void AddService_LazilySingleDependencyThere_ResolvesInstance()
    {
      _machineContainer.AddService<Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_machineContainer.Resolve<IService2>());
    }

    [Test]
    [ExpectedException(typeof(AmbiguousServicesException))]
    public void AddService_LazilyMultipleDependencies_ThrowsAmbiguous()
    {
      _machineContainer.AddService<Service1DependsOn2>(LifestyleType.Singleton);
      _machineContainer.AddService<SimpleService1>(LifestyleType.Singleton);
      _machineContainer.Resolve<IService1>();
    }

    [Test]
    public void AddService_CircularDependency_Throws()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<IService1, Service1DependsOn2>(LifestyleType.Singleton);
    }

    [Test]
    [ExpectedException(typeof(ServiceResolutionException))]
    public void AddService_Duplicate_Throws()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<IService2, Service1DependsOn2>(LifestyleType.Singleton);
    }

    [Test]
    [ExpectedException(typeof(CircularDependencyException))]
    public void Resolve_CircularDependency_Throws()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<IService1, Service1DependsOn2>(LifestyleType.Singleton);
      _machineContainer.Resolve<IService2>();
    }

    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void Resolve_WaitingDependencies_Throws()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.Resolve<IService2>();
    }

    [Test]
    public void Resolve_NotWaitingDependencies_Works()
    {
      _machineContainer.AddService<IService2, Service2DependsOn1>(LifestyleType.Singleton);
      _machineContainer.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      Assert.IsNotNull(_machineContainer.Resolve<IService2>());
    }

    [Test]
    public void Resolve_Singleton_YieldsSameInstances()
    {
      _machineContainer.AddService<IService1, SimpleService1>(LifestyleType.Singleton);
      IService1 service1 = _machineContainer.Resolve<IService1>();
      IService1 service2 = _machineContainer.Resolve<IService1>();
      Assert.AreEqual(service1, service2);
    }

    [Test]
    public void Resolve_Transient_YieldsMultipleInstances()
    {
      _machineContainer.AddService<IService1, SimpleService1>(LifestyleType.Transient);
      IService1 service1 = _machineContainer.Resolve<IService1>();
      IService1 service2 = _machineContainer.Resolve<IService1>();
      Assert.AreNotEqual(service1, service2);
    }

    [Test]
    public void ResolveWithOverrides_WithOverrides_UsesOverride()
    {
      _machineContainer.AddService<IService1, Service1DependsOn2>(LifestyleType.Transient);
      Assert.IsNotNull(_machineContainer.ResolveWithOverrides<IService1>(new SimpleService2()));
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
