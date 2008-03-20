using System;
using System.Collections.Generic;
using System.ComponentModel;

using Castle.DynamicProxy;

namespace Machine.Core.Aspects
{
  public class ViewProxyGenerator
  {
    private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

    public TView CreateViewProxy<TView>(TView view, ISynchronizeInvoke synchronizeInvoke)
    {
      return _proxyGenerator.CreateInterfaceProxyWithTarget<TView>(view, new SynchronizeInvokeInterceptor(synchronizeInvoke));
    }
  }
}