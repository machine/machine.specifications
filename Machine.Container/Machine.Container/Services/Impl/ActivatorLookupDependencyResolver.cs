using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ActivatorLookupDependencyResolver : IDependencyResolver
  {
    #region IDependencyResolver Members
    public IActivator ResolveDependency(ICreationServices services, ServiceEntry serviceEntry)
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
