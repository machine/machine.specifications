using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using NUnit.Framework;

namespace Castle.Facilities.DeferredServiceResolution
{
  [TestFixture]
  public class DeferredContainerTests : StandardFixture<DeferredContainer>
  {
    [Test]
    public void Resolve_JustService1ByImplementation_GetsService()
    {
      _target.AddComponent("Service1", typeof(Service1));
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<Service1>());
    }

    [Test]
    public void Resolve_JustService1ByInterface_GetsService()
    {
      _target.AddComponent("Service1", typeof(Service1));
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<IService1>());
    }

    [Test]
    public void Resolve_Service2DependsOnService1ByImplementation_GetsService()
    {
      _target.AddComponent("Service1", typeof(Service1));
      _target.AddComponent("Service2DependsOnService1", typeof(Service2DependsOnService1));
      Assert.IsInstanceOfType(typeof(Service2DependsOnService1), _target.Resolve<Service2DependsOnService1>());
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<Service1>());
    }

    [Test]
    public void Resolve_Service2DependsOnService1ByInterface_GetsService()
    {
      _target.AddComponent("Service1", typeof(Service1));
      _target.AddComponent("Service2DependsOnService1", typeof(Service2DependsOnService1));
      Assert.IsInstanceOfType(typeof(Service2DependsOnService1), _target.Resolve<IService2>());
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<IService1>());
    }

    [Test]
    public void Resolve_TwoService1DependentsThatMightCauseItToNotCheckAgainAndResolveByImplementation_GetsService()
    {
      _target.AddComponent("Service2DependsOnService1", typeof(Service2DependsOnService1));
      _target.AddComponent("Service3DependsOnService1And2", typeof(Service3DependsOnService1And2));
      _target.AddComponent("Service1", typeof(Service1));
      Assert.IsInstanceOfType(typeof(Service2DependsOnService1), _target.Resolve<Service2DependsOnService1>());
      Assert.IsInstanceOfType(typeof(Service3DependsOnService1And2), _target.Resolve<Service3DependsOnService1And2>());
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<Service1>());
    }

    [Test]
    public void Resolve_TwoService1DependentsThatMightCauseItToNotCheckAgainAndResolveByInterface_GetsService()
    {
      _target.AddComponent("Service2DependsOnService1", typeof(Service2DependsOnService1));
      _target.AddComponent("Service3DependsOnService1And2", typeof(Service3DependsOnService1And2));
      _target.AddComponent("Service1", typeof(Service1));
      Assert.IsInstanceOfType(typeof(Service2DependsOnService1), _target.Resolve<IService2>());
      Assert.IsInstanceOfType(typeof(Service3DependsOnService1And2), _target.Resolve<IService3>());
      Assert.IsInstanceOfType(typeof(Service1), _target.Resolve<IService1>());
    }

    public override DeferredContainer Create()
    {
      DeferredContainer container = new DeferredContainer();
      container.AddDeferredFacility();
      return container;
    }
  }
}
