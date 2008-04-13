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
    private readonly MockRepository _mocks;
    private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
    #endregion

    #region MockingDependencyResolver()
    public MockingDependencyResolver(MockRepository mocks)
    {
      _mocks = mocks;
    }
    #endregion

    #region IActivatorResolver Members
    public IActivator ResolveActivator(ICreationServices services, ServiceEntry serviceEntry)
    {
      if (serviceEntry.ServiceType.IsInterface)
      {
        return services.ActivatorStrategy.CreateStaticActivator(serviceEntry, Get(serviceEntry.ServiceType));
      }
      return null;
    }
    #endregion

    #region Methods
    public TService Get<TService>()
    {
      return (TService)Get(typeof (TService));
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
