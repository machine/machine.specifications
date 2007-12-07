using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.MicroKernel;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class DeferredServiceResolver : ISubDependencyResolver
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(DeferredServiceResolver));
    #endregion

    #region Member Data
    private readonly IComponentModelAndKernelSource _componentModelSource;
    private readonly IServiceInterfaceResolver _serviceInterfaceResolver;
    #endregion

    #region DeferredServiceResolver()
    public DeferredServiceResolver(IComponentModelAndKernelSource componentModelSource, IServiceInterfaceResolver serviceInterfaceResolver)
    {
      _componentModelSource = componentModelSource;
      _serviceInterfaceResolver = serviceInterfaceResolver;
    }
    #endregion

    #region ISubDependencyResolver Members
    public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
    {
      try
      {
        return _serviceInterfaceResolver.AttemptResolve(dependency.TargetType, _componentModelSource.Models) != null;
      }
      catch (Exception error)
      {
        throw new ServiceResolutionException(String.Format("Error resolving {0}", model.Name), error);
      }
    }

    public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
    {
      try
      {
        ComponentModel resolved = _serviceInterfaceResolver.Resolve(dependency.TargetType, _componentModelSource.Models);
        return _componentModelSource.Kernel[resolved.Service];
      }
      catch (Exception error)
      {
        throw new ServiceResolutionException(String.Format("Error resolving {0}", model.Name), error);
      }
    }
    #endregion
  }
}