using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class DefaultLifestyleAwareActivatorResolver : IActivatorResolver
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(DefaultLifestyleAwareActivatorResolver));
    #endregion

    #region IActivatorResolver Members
    public IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry)
    {
      _log.Info("ResolveActivator: " + serviceEntry);
      if (!services.ActivatorStore.HasActivator(serviceEntry))
      {
        return null;
      }
      IActivator activator = services.ActivatorStore.ResolveActivator(serviceEntry);
      if (activator.CanActivate(services))
      {
        return activator;
      }
      return null;
    }
    #endregion
  }
}
