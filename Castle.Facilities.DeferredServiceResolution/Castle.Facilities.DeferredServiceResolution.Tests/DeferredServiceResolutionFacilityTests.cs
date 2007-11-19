using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.Core.Configuration;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Castle.Facilities.DeferredServiceResolution
{
  [TestFixture]
  public class DeferredServiceResolutionFacilityTests : StandardFixture<DeferredServiceResolutionFacility>
  {
    private IEventRaiser _componentModelCreated;

    [Test]
    public void Init_Always_SetsKernel()
    {
      IConfiguration configuration = _mocks.DynamicMock<IConfiguration>();
      using (_mocks.Record())
      {
        SetupInit();
      }
      _target.Init(_kernel, configuration);
      Assert.AreEqual(_kernel, _target.Kernel);
    }

    [Test]
    public void Init_Always_RegisteredComponentModelCreatedListeners()
    {
      IConfiguration configuration = _mocks.DynamicMock<IConfiguration>();
      using (_mocks.Record())
      {
        SetupInit();
      }
      _target.Init(_kernel, configuration);
      _mocks.VerifyAll();
    }

    [Test]
    public void Init_Always_AddsSubResolver()
    {
      IConfiguration configuration = _mocks.DynamicMock<IConfiguration>();
      using (_mocks.Record())
      {
        SetupInit();
        _dependencyResolver.AddSubResolver(_target.DeferredServiceResolver);
      }
      _target.Init(_kernel, configuration);
      _mocks.VerifyAll();
    }

    [Test]
    public void CreatingComponentModel_Always_RemembersTheModel()
    {
      ComponentModel model = new ComponentModel("Model", typeof(Random), typeof(Random));
      IConfiguration configuration = _mocks.DynamicMock<IConfiguration>();
      using (_mocks.Record())
      {
        SetupInit();
      }
      _target.Init(_kernel, configuration);
      _componentModelCreated.Raise(model);
      _mocks.VerifyAll();
      CollectionAssert.AreEqual(new ComponentModel[] { model }, _target.Models);
    }

    [Test]
    public void Terminate_Always_DoesNothing()
    {
      _target.Terminate();
    }

    public void SetupInit()
    {
      SetupResult.For(_kernel.Resolver).Return(_dependencyResolver);
      _kernel.ComponentModelCreated += null;
      _componentModelCreated = LastCall.IgnoreArguments().GetEventRaiser();
    }

    public override DeferredServiceResolutionFacility Create()
    {
      return new DeferredServiceResolutionFacility();
    }
  }
}
