using System;
using System.Collections.Generic;

using Castle.DynamicProxy;

using Machine.Container.Model;
using Machine.Container.Plugins;
using Machine.Container.Services;
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
      IActivatorFactory defaultActivatorFactory = services.Factory.FindChainedItemByType(typeof(DefaultActivatorFactory));
      services.Factory.AddBefore(typeof(DefaultActivatorFactory), new DynamicProxyActivatorFactory(services.Container, _proxyBuilder, defaultActivatorFactory));
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
    }
    #endregion
  }
}
