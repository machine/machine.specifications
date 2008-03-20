using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class LifestyleStore : ILifestyleStore
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LifestyleStore));
    #endregion

    #region Member Data
    private readonly Dictionary<ServiceEntry, ILifestyle> _cache = new Dictionary<ServiceEntry, ILifestyle>();
    private readonly ILifestyleFactory _lifestyleFactory;
    #endregion

    #region LifestyleStore()
    public LifestyleStore(ILifestyleFactory lifestyleFactory)
    {
      _lifestyleFactory = lifestyleFactory;
    }
    #endregion

    #region ILifestyleStore Members
    public ILifestyle ResolveLifestyle(ServiceEntry entry)
    {
      if (_cache.ContainsKey(entry))
      {
        return _cache[entry];
      }
      _log.Info("Creating On Demand: " + entry);
      return _cache[entry] = _lifestyleFactory.CreateLifestyle(entry);
    }

    public void AddLifestyle(ServiceEntry entry, ILifestyle lifestyle)
    {
      _log.Info("Adding: " + entry + " " + lifestyle);
      _cache[entry] = lifestyle;
    }
    #endregion
  }
}
