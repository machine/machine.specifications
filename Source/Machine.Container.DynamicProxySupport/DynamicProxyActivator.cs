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
    private readonly ServiceEntry _entry;

    public DynamicProxyActivator(IMachineContainer container, IProxyBuilder proxyBuilder, IActivator target, ServiceEntry entry)
    {
      _container = container;
      _entry = entry;
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
      Activation activation = _target.Activate(services);
      ServiceEntry entry = activation.Entry;
      if (!entry.HasInterceptorsApplied)
      {
        throw new ServiceContainerException("Entry should have interceptors: " + entry);
      }
      try
      {
        object proxied = _proxyGenerator.CreateInterfaceProxyWithTarget(entry.ServiceType, activation.Instance, ProxyGenerationOptions.Default, FindInterceptors(entry));
        return new Activation(activation.Entry, proxied);
      }
      catch (Exception error)
      {
        throw new DynamicProxyException("Error activating with DynamicProxy", error);
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
        try
        {
          interceptors.Add((IInterceptor)_container.Resolve.Object(interceptor.InterceptorType));
        }
        catch (Exception error)
        {
          throw new DynamicProxyException("Error resolving Interceptor: " + interceptor, error);
        }
      }
      return interceptors.ToArray();
    }
  }
}