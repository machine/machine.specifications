using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class DeferredServiceResolutionFacility : IFacility, IComponentModelAndKernelSource
  {
    private readonly List<ComponentModel> _models;
    private readonly DeferredServiceResolver _deferredServiceResolver;
    private readonly IServiceInterfaceResolver _serviceInterfaceResolver;
    private readonly IServiceResolver _serviceResolver;
    private IKernel _kernel;

    public List<ComponentModel> Models
    {
      get { return _models; }
    }

    public IKernel Kernel
    {
      get { return _kernel; }
    }

    public DeferredServiceResolver DeferredServiceResolver
    {
      get { return _deferredServiceResolver; }
    }

    public IServiceInterfaceResolver ServiceInterfaceResolver
    {
      get { return _serviceInterfaceResolver; }
    }

    public IServiceResolver ServiceResolver
    {
      get { return _serviceResolver; }
    }

    public DeferredServiceResolutionFacility()
    {
      _models = new List<ComponentModel>();
      _serviceInterfaceResolver = new ServiceInterfaceResolver();
      _deferredServiceResolver = new DeferredServiceResolver(this, _serviceInterfaceResolver);
      _serviceResolver = new ServiceResolver(this, _serviceInterfaceResolver);
    }

    #region IFacility Members
    public void Init(IKernel kernel, IConfiguration facilityConfig)
    {
      _kernel = kernel;
      kernel.ComponentModelCreated += OnComponentModelCreated;
      kernel.Resolver.AddSubResolver(_deferredServiceResolver);
    }

    public void Terminate()
    {
    }
    #endregion

    private void OnComponentModelCreated(ComponentModel model)
    {
      _models.Add(model);
    }
  }
}
