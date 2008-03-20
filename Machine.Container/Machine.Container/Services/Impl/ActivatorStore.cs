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
    private readonly IActivatorStrategy _activatorStrategy;
    private readonly ILifestyleFactory _lifestyleFactory;
    #endregion

    #region ActivatorStore()
    public ActivatorStore(IActivatorStrategy activatorStrategy, ILifestyleFactory lifestyleFactory)
    {
      _activatorStrategy = activatorStrategy;
      _lifestyleFactory = lifestyleFactory;
    }
    #endregion

    #region IActivatorStore Members
    public IActivator ResolveActivator(ServiceEntry entry)
    {
      if (_cache.ContainsKey(entry))
      {
        return _cache[entry];
      }
      _log.Info("Creating On Demand: " + entry);
      ILifestyle lifestyle = _lifestyleFactory.CreateLifestyle(entry);
      return _cache[entry] = _activatorStrategy.CreateLifestyleActivator(lifestyle);
    }

    public void AddActivator(ServiceEntry entry, IActivator activator)
    {
      _log.Info("Adding: " + entry + " " + activator);
      _cache[entry] = activator;
    }
    #endregion
  }
}
