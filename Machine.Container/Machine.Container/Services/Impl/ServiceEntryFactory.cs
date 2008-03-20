using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ServiceEntryFactory : IServiceEntryFactory
  {
    #region IServiceEntryFactory Members
    public ServiceEntry CreateServiceEntry(Type serviceType, Type implementationType, LifestyleType lifestyleType)
    {
      return new ServiceEntry(serviceType, implementationType, lifestyleType);
    }
    #endregion
  }
}