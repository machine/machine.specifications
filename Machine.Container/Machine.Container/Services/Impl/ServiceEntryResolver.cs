using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ServiceEntryResolver : IServiceEntryResolver
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServiceEntryResolver));
    #endregion

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
      if (entry.ImplementationType != implementationType && serviceType != implementationType)
      {
        throw new ServiceResolutionException("Can't add a service twice with two implementations: " + serviceType);
      }
      return entry;
    }

    public ResolvedServiceEntry ResolveEntry(ICreationServices services, Type serviceType)
    {
      _log.Info("ResolveEntry: " + serviceType);
      ServiceEntry entry = _serviceGraph.Lookup(serviceType);
      if (entry == null)
      {
        entry = _serviceEntryFactory.CreateServiceEntry(serviceType, serviceType, LifestyleType.Transient);
      }
      IActivator activator = _activatorResolver.ResolveActivator(services, entry);
      if (activator != null)
      {
        return new ResolvedServiceEntry(entry, activator);
      }
      return null;
    }
    #endregion
  }
}