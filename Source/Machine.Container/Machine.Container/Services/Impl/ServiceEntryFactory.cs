using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ServiceEntryFactory : IServiceEntryFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServiceEntryFactory));
    #endregion

    #region IServiceEntryFactory Members
    public ServiceEntry CreateServiceEntry(Type serviceType, Type implementationType, LifestyleType lifestyleType)
    {
      ServiceEntry entry = new ServiceEntry(serviceType, implementationType, lifestyleType);
      _log.Info("Creating: " + entry);
      return entry;
    }
    #endregion
  }
}