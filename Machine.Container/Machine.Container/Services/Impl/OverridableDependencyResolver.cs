using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class OverridableDependencyResolver : IDependencyResolver
  {
    #region IDependencyResolver Members
    public IActivator ResolveDependency(ICreationServices services, ServiceEntry serviceEntry)
    {
      object value = services.Overrides.LookupOverride(serviceEntry);
      if (value == null)
      {
        return null;
      }
      return services.ActivatorStrategy.CreateStaticActivator(serviceEntry, value);
    }
    #endregion
  }
}
