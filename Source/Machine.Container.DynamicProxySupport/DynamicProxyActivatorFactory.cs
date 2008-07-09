using Castle.DynamicProxy;

using Machine.Container.Model;
using Machine.Container.Services;

namespace Machine.Container.DynamicProxySupport
{
  public class DynamicProxyActivatorFactory : IActivatorFactory
  {
    private readonly IActivatorFactory _defaultActivatorFactory;
    private readonly IMachineContainer _container;
    private readonly IProxyBuilder _proxyBuilder;

    public DynamicProxyActivatorFactory(IMachineContainer container, IProxyBuilder proxyBuilder, IActivatorFactory defaultActivatorFactory)
    {
      _container = container;
      _defaultActivatorFactory = defaultActivatorFactory;
      _proxyBuilder = proxyBuilder;
    }

    #region IActivatorFactory Members
    public IActivator CreateStaticActivator(ServiceEntry entry, object instance)
    {
      return null;
    }

    public IActivator CreateDefaultActivator(ServiceEntry entry)
    {
      if (!entry.HasInterceptorsApplied)
      {
        return null;
      }
      return new DynamicProxyActivator(_container, _proxyBuilder, _defaultActivatorFactory.CreateDefaultActivator(entry), entry);
    }
    #endregion
  }
}