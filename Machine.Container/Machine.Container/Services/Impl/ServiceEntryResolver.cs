using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Utility;

namespace Machine.Container.Services.Impl
{
  public class ServiceEntryResolver : IServiceEntryResolver
  {
    #region Member Data
    private readonly IServiceGraph _serviceGraph;
    private readonly IServiceEntryFactory _serviceEntryFactory;
    private readonly IActivatorResolver _activatorResolver;
    #endregion

    #region ServiceEntryResolver()
    public ServiceEntryResolver(IServiceGraph serviceGraph, IServiceEntryFactory serviceEntryFactory, IActivatorResolver activatorResolver)
    {
      _serviceGraph = serviceGraph;
      _activatorResolver = activatorResolver;
      _serviceEntryFactory = serviceEntryFactory;
    }
    #endregion

    #region Methods
    public ServiceEntry CreateEntryIfMissing(Type serviceType)
    {
      return CreateEntryIfMissing(serviceType, serviceType);
    }

    public ServiceEntry CreateEntryIfMissing(Type serviceType, Type implementationType)
    {
      ServiceEntry entry = _serviceGraph.Lookup(serviceType);
      if (entry == null)
      {
        entry = _serviceEntryFactory.CreateServiceEntry(serviceType, implementationType, LifestyleType.Singleton);
        _serviceGraph.Add(entry);
      }
      if (entry.ImplementationType != implementationType)
      {
        throw new ServiceResolutionException("Can't add a service twice with two implementations: " + serviceType);
      }
      return entry;
    }

    public ServiceEntry ResolveEntry(ICreationServices services, Type serviceType)
    {
      return ResolveActivator(services, serviceType);
    }
    #endregion

    #region Methods
    protected virtual ServiceEntry ResolveActivator(ICreationServices services, Type serviceType)
    {
      ServiceEntry entry = _serviceGraph.LookupLazily(serviceType);
      if (entry == null)
      {
        entry = _serviceEntryFactory.CreateServiceEntry(serviceType, serviceType, LifestyleType.Transient);
      }
      if (entry.Activator != null)
      {
        return entry;
      }
      IActivator activator = _activatorResolver.ResolveActivator(services, entry);
      if (activator != null)
      {
        entry.Activator = activator;
        return entry;
      }
      return null;
    }
    #endregion
  }
}