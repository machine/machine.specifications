using System;
using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class ServiceResolver : IServiceResolver
  {
    #region Member Data
    private readonly IComponentModelAndKernelSource _componentModelAndKernelSource;
    private readonly IServiceInterfaceResolver _serviceInterfaceResolver;
    #endregion

    #region ServiceResolver()
    public ServiceResolver(IComponentModelAndKernelSource componentModelAndKernelSource, IServiceInterfaceResolver serviceInterfaceResolver)
    {
      _componentModelAndKernelSource = componentModelAndKernelSource;
      _serviceInterfaceResolver = serviceInterfaceResolver;
    }
    #endregion

    #region IServiceResolver Members
    public T Resolve<T>()
    {
      ComponentModel resolved = ResolveModel(typeof(T));
      return (T)_componentModelAndKernelSource.Kernel[resolved.Service];
    }

    public ComponentModel ResolveModel(Type type)
    {
      return _serviceInterfaceResolver.Resolve(type, _componentModelAndKernelSource.Models, false);
    }

    public bool CanResolve(Type type)
    {
      return _serviceInterfaceResolver.Resolve(type, _componentModelAndKernelSource.Models, false) != null;
    }
    #endregion
  }
}