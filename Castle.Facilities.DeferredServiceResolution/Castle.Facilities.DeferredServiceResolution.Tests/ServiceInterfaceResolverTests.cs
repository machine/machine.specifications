using System;
using System.Collections.Generic;

using Castle.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Facilities.DeferredServiceResolution
{
  [TestFixture]
  public class ServiceInterfaceResolverTests : StandardFixture<ServiceInterfaceResolver>
  {
    [Test]
    public void Resolve_Service1WithNoServices_Fails()
    {
      Assert.IsNull(_target.Resolve(typeof(IService1), ComponentModels.WithNoServices(), false));
    }

    [Test]
    public void Resolve_Service1WithService1_IsRightModel()
    {
      Assert.AreEqual(ComponentModels.Service1Model, _target.Resolve(typeof(IService1), ComponentModels.WithService1And2(), false));
    }

    [Test]
    public void Resolve_Service3WithoutService3ButHasOther_Fails()
    {
      Assert.IsNull(_target.Resolve(typeof(IService3), ComponentModels.WithService1And2(), false));
    }

    [Test]
    public void Resolve_Service1WhenHasDuplicateImplementors_Fails()
    {
      Assert.IsNull(_target.Resolve(typeof(IService1), ComponentModels.WithDuplicateService1(), false));
    }

    [Test]
    public void Resolve_Service2WhenItsOneOfTwoImplementations_IsRightModel()
    {
      Assert.AreEqual(ComponentModels.Service1And2Model, _target.Resolve(typeof(IService2), ComponentModels.WithDuplicateService1(), false));
    }

    [Test]
    public void Resolve_ServiceByClassImplementationNotServiceAndIsThere_IsRightModel()
    {
      Assert.AreEqual(ComponentModels.NoServicesModel, _target.Resolve(typeof(NoServices), ComponentModels.WithService1And2And3(), false));
    }

    public override ServiceInterfaceResolver Create()
    {
      return new ServiceInterfaceResolver();
    }
  }
}
