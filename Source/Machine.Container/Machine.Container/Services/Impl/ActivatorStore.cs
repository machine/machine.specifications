using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ActivatorStore : IActivatorStore
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ActivatorStore));
    #endregion

    #region Member Data
    private readonly Dictionary<ServiceEntry, IActivator> _cache = new Dictionary<ServiceEntry, IActivator>();
    #endregion

    #region IActivatorStore Members
    public IActivator ResolveActivator(ServiceEntry entry)
    {
      return _cache[entry];
    }

    public void AddActivator(ServiceEntry entry, IActivator activator)
    {
      _log.Info("Adding: " + entry + " " + activator);
      _cache[entry] = activator;
    }

    public bool HasActivator(ServiceEntry entry)
    {
      return _cache.ContainsKey(entry);
    }
    #endregion
  }
}
