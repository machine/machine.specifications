using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ActivatorLookupDependencyResolver : IActivatorResolver
  {
    #region IActivatorResolver Members
    public IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry)
    {
      IActivator activator =  services.ActivatorStore.ResolveActivator(serviceEntry);
      if (activator.CanActivate(services))
      {
        return activator;
      }
      return null;
    }
    #endregion
  }
}
