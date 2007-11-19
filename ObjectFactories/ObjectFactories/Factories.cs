using System;
using System.Collections.Generic;

using ObjectFactories.Services;
using ObjectFactories.Services.Impl;

namespace ObjectFactories
{
  public static class Factories
  {
    private static ISuperFactory _superFactory;

    public static ISuperFactory SuperFactory
    {
      get
      {
        if (_superFactory == null)
        {
          _superFactory = new DefaultSuperFactory();
        }
        return _superFactory;
      }
      set
      {
        _superFactory = value;
      }
    }

    public static IObjectFactory<TType> GetFactory<TType>()
    {
      return SuperFactory.CreateFactory<TType>();
    }

    public static TType Create<TType>()
    {
      return GetFactory<TType>().Create();
    }
  }
}
