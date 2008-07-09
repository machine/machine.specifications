using System;
using System.Collections.Generic;

using Castle.DynamicProxy;

using Machine.Container.Model;
using Machine.Container.Plugins;
using Machine.Container.Services.Impl;

namespace Machine.Container.DynamicProxySupport
{
  public class DynamicProxyPlugin : IServiceContainerPlugin
  {
    private readonly IProxyBuilder _proxyBuilder = new DefaultProxyBuilder();

    #region IServiceContainerPlugin Members
    public void Initialize(PluginServices services)
    {
      services.StatePolicy.AddSupportedFeature(SupportedFeature.Interceptors);
      services.Resolver.Replace(typeof(ActivatorStoreActivatorResolver), new DynamicProxyActivatorResolver(services.Container, _proxyBuilder));
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
    }
    #endregion
  }
}
