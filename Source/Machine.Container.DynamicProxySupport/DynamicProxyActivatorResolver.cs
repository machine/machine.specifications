using Castle.DynamicProxy;

using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

namespace Machine.Container.DynamicProxySupport
{
  public class DynamicProxyActivatorResolver : ActivatorStoreActivatorResolver
  {
    private readonly IMachineContainer _container;
    private readonly IProxyBuilder _proxyBuilder;

    public DynamicProxyActivatorResolver(IMachineContainer container, IProxyBuilder proxyBuilder)
    {
      _container = container;
      _proxyBuilder = proxyBuilder;
    }

    #region IActivatorResolver Members
    public override IActivator ResolveActivator(IResolutionServices services, ServiceEntry entry)
    {
      IActivator activator = base.ResolveActivator(services, entry);
      if (!entry.HasInterceptors)
      {
        return activator;
      }
      return new DynamicProxyActivator(_container, _proxyBuilder, activator);
    }
    #endregion
  }
}