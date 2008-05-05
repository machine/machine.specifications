using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ServiceGraph : IServiceGraph
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServiceGraph));
    #endregion

    #region Member Data
    private readonly Dictionary<Type, ServiceEntry> _map = new Dictionary<Type, ServiceEntry>();
    #endregion

    #region IServiceGraph Members
    public ServiceEntry Lookup(Type type, bool throwIfAmbiguous)
    {
      return LookupLazily(type, throwIfAmbiguous);
    }

    public ServiceEntry Lookup(Type type)
    {
      return LookupLazily(type, true);
    }

    public void Add(ServiceEntry entry)
    {
      _log.Info("Adding: " + entry);
      _map[entry.ServiceType] = entry;
    }

    public IEnumerable<ServiceRegistration> RegisteredServices
    {
      get
      {
        foreach (ServiceEntry serviceEntry in _map.Values)
        {
          yield return new ServiceRegistration(serviceEntry.ServiceType, serviceEntry.ImplementationType);
        }
      }
    }
    #endregion

    public ServiceEntry LookupLazily(Type type, bool throwIfAmbiguous)
    {
      List<ServiceEntry> matches = new List<ServiceEntry>();
      foreach (ServiceEntry entry in _map.Values)
      {
        if (type.IsAssignableFrom(entry.ServiceType))
        {
          matches.Add(entry);
        }
      }
      if (matches.Count == 1)
      {
        return matches[0];
      }
      else if (matches.Count > 1 && throwIfAmbiguous)
      {
        throw new AmbiguousServicesException(type.ToString());
      }
      return null;
    }
  }
}