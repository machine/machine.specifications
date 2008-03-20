using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class StaticOverrideLookup : IOverrideLookup
  {
    #region Member Data
    private readonly object[] _objects;
    #endregion

    #region StaticOverrideLookup()
    public StaticOverrideLookup(object[] objects)
    {
      _objects = objects;
    }
    #endregion

    #region IOverrideLookup Members
    public object LookupOverride(ServiceEntry serviceEntry)
    {
      foreach (object service in _objects)
      {
        if (serviceEntry.ServiceType.IsAssignableFrom(service.GetType()))
        {
          return service;
        }
      }
      return null;
    }
    #endregion
  }
}
