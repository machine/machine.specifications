using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Machine.Container.Model;
using Machine.Container.Services;

namespace Machine.MsMvc
{
  public class ReflectiveControllerTypeMap : IControllerTypeMap
  {
    private readonly IHighLevelContainer _container;
    private Dictionary<string, Type> _map;

    public ReflectiveControllerTypeMap(IHighLevelContainer container)
    {
      _container = container;
    }

    #region IControllerTypeMap Members
    public Type LookupControllerType(string controllerName)
    {
      if (_map == null)
      {
        _map = CreateMap();
      }
      if (!_map.ContainsKey(controllerName))
      {
        throw new KeyNotFoundException("No controller named: " + controllerName);
      }
      return _map[controllerName];
    }
    #endregion

    private Dictionary<string, Type> CreateMap()
    {
      Dictionary<string, Type> map = new Dictionary<string, Type>();
      foreach (ServiceRegistration registration in _container.RegisteredServices)
      {
        if (IsController(registration.ServiceType))
        {
          map[GetControllerName(registration.ServiceType)] = registration.ServiceType;
        }
      }
      return map;
    }

    protected virtual bool IsController(Type type)
    {
      return typeof(IController).IsAssignableFrom(type);
    }

    protected virtual string GetControllerName(Type type)
    {
      string name = type.Name;
      if (name.EndsWith("Controller"))
      {
        name = name.Substring(0, name.Length - "Controller".Length);
      }
      return name;
    }
  }
}
