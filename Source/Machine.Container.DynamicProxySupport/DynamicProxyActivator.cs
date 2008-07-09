using System;
using System.Collections.Generic;

using Castle.Core.Interceptor;
using Castle.DynamicProxy;

using Machine.Container.Model;
using Machine.Container.Services;

namespace Machine.Container.DynamicProxySupport
{
  public class DynamicProxyActivator : IActivator
  {
    private readonly IMachineContainer _container;
    private readonly IProxyBuilder _proxyBuilder;
    private readonly IActivator _target;
    private readonly ProxyGenerator _proxyGenerator;

    public DynamicProxyActivator(IMachineContainer container, IProxyBuilder proxyBuilder, IActivator target)
    {
      _container = container;
      _proxyBuilder = proxyBuilder;
      _target = target;
      _proxyGenerator = new ProxyGenerator(_proxyBuilder);
    }

    #region IActivator Members
    public bool CanActivate(IResolutionServices services)
    {
      return _target.CanActivate(services);
    }

    public Activation Activate(IResolutionServices services)
    {
      try
      {
        Activation activation = _target.Activate(services);
        if (!activation.Entry.HasInterceptors)
        {
          throw new ServiceContainerException("You should have interceptors!");
        }
        ServiceEntry entry = activation.Entry;
        object proxied = _proxyGenerator.CreateInterfaceProxyWithTarget(entry.ServiceType, activation.Instance, ProxyGenerationOptions.Default, FindInterceptors(entry));
        return new Activation(activation.Entry, proxied);
      }
      catch (Exception ex)
      {
        throw new ServiceContainerException("Error activating with DynamicProxy", ex);
      }
    }

    public void Deactivate(IResolutionServices services, object instance)
    {
      _target.Deactivate(services, instance);
    }
    #endregion

    protected virtual IInterceptor[] FindInterceptors(ServiceEntry entry)
    {
      List<IInterceptor> interceptors = new List<IInterceptor>();
      foreach (InterceptorApplication interceptor in entry.Interceptors)
      {
        interceptors.Add((IInterceptor)_container.Resolve.Object(interceptor.InterceptorType));
      }
      return interceptors.ToArray();
    }
  }
}