using System;
using System.Collections.Generic;

using Machine.Container.Activators;
using Machine.Container.Model;
using Machine.Container.Services;

namespace Machine.Container.Services.Impl
{
  public class DefaultActivatorStrategy : IActivatorStrategy
  {
    #region Member Data
    private readonly IObjectFactory _objectFactory;
    private readonly IServiceEntryResolver _serviceEntryResolver;
    private readonly IServiceDependencyInspector _serviceDependencyInspector;
    #endregion

    #region DefaultActivatorStrategy()
    public DefaultActivatorStrategy(IObjectFactory objectFactory, IServiceEntryResolver serviceEntryResolver, IServiceDependencyInspector serviceDependencyInspector)
    {
      _objectFactory = objectFactory;
      _serviceDependencyInspector = serviceDependencyInspector;
      _serviceEntryResolver = serviceEntryResolver;
    }
    #endregion

    #region IActivatorStrategy Members
    public IActivator CreateLifestyleActivator(ILifestyle lifestyle)
    {
      return new LifestyleActivator(lifestyle);
    }

    public IActivator CreateStaticActivator(ServiceEntry entry, object instance)
    {
      return new StaticActivator(entry, instance);
    }

    public IActivator CreateDefaultActivator(ServiceEntry entry)
    {
      return new DefaultActivator(_objectFactory, _serviceDependencyInspector, _serviceEntryResolver, entry);
    }
    #endregion
  }
}