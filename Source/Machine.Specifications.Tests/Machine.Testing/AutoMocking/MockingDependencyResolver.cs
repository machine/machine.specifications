using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services;

using Rhino.Mocks;

namespace Machine.Testing.AutoMocking
{
  public class MockingDependencyResolver : IActivatorResolver
  {
    #region Member Data
    readonly MockRepository _mocks;
    readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
    #endregion

    #region MockingDependencyResolver()
    public MockingDependencyResolver(MockRepository mocks)
    {
      _mocks = mocks;
    }
    #endregion

    #region IActivatorResolver Members
    public IActivator ResolveActivator(IResolutionServices services, ServiceEntry entry)
    {
      if (entry.ServiceType.IsInterface)
      {
        return services.ActivatorFactory.CreateStaticActivator(entry, Get(entry.ServiceType));
      }
      return null;
    }
    #endregion

    #region Methods
    public TService Get<TService>()
    {
      return (TService)Get(typeof(TService));
    }

    public object Get(Type serviceType)
    {
      if (!_objects.ContainsKey(serviceType))
      {
        _objects[serviceType] = _mocks.DynamicMock(serviceType);
      }
      return _objects[serviceType];
    }
    #endregion
  }
}