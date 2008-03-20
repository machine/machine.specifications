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

    #region Methods
    public ServiceEntry Lookup(Type type)
    {
      if (_map.ContainsKey(type))
      {
        return _map[type];
      }
      return null;
    }

    public ServiceEntry LookupLazily(Type type)
    {
      foreach (ServiceEntry entry in _map.Values)
      {
        if (type.IsAssignableFrom(entry.ConcreteType))
        {
          return entry;
        }
      }
      return null;
    }

    public void Add(ServiceEntry entry)
    {
      _log.Info("Adding: " + entry);
      _map[entry.ServiceType] = entry;
    }
    #endregion
  }
}