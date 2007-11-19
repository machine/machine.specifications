using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.MicroKernel;

using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Facilities.DeferredServiceResolution
{
  [TestFixture]
  public class DeferredServiceResolverTests : StandardFixture<DeferredServiceResolver>
  {
    private IComponentModelAndKernelSource _componentModelSource;
    private IServiceInterfaceResolver _serviceInterfaceResolver;
    private List<ComponentModel> _models;
    private CreationContext _context;
    private ISubDependencyResolver _parentResolver;
    private ComponentModel _model;
    private DependencyModel _dependency;

    [Test]
    public void CanResolve__()
    {
      using (_mocks.Record())
      {
        SetupModelSource();
      }
      Assert.IsFalse(_target.CanResolve(_context, _parentResolver, _model, _dependency));
    }

    public override DeferredServiceResolver Create()
    {
      _componentModelSource = _mocks.DynamicMock<IComponentModelAndKernelSource>();
      _serviceInterfaceResolver = _mocks.DynamicMock<IServiceInterfaceResolver>();
      _models = new List<ComponentModel>();
      _context = new CreationContext(new DependencyModel[0]);
      _model = new ComponentModel("TargetModel", typeof(Random), typeof(Random));
      _parentResolver = null;
      _dependency = new DependencyModel(DependencyType.Service, "ServiceProvider", typeof(IServiceProvider), false);
      return new DeferredServiceResolver(_componentModelSource, _serviceInterfaceResolver);
    }

    public void SetupModelSource()
    {
      SetupResult.For(_componentModelSource.Models).Return(_models);
    }
  }
}
