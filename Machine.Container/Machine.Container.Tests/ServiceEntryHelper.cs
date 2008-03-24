using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services.Impl;

namespace Machine.Container
{
  public static class ServiceEntryHelper
  {
    #region Member Data
    public static ServiceEntry NewEntry()
    {
      return NewEntry(typeof(IService1), typeof(Service1DependsOn2));
    }

    public static ServiceEntry NewEntry(Type serviceType)
    {
      return new ServiceEntry(serviceType, serviceType, LifestyleType.Singleton);
    }

    public static ServiceEntry NewEntry(Type serviceType, Type implementationTyoe)
    {
      return new ServiceEntry(serviceType, implementationTyoe, LifestyleType.Singleton);
    }

    public static ServiceEntry NewEntry(LifestyleType lifestyleType)
    {
      return new ServiceEntry(typeof(IService1), typeof(Service1DependsOn2), lifestyleType);
    }
    #endregion
  }
}
