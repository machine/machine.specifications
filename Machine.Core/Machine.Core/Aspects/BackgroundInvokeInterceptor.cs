using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

using Castle.Core.Interceptor;

namespace Machine.Core.Aspects
{
  public class BackgroundInvokeInterceptor : IInterceptor
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BackgroundInvokeInterceptor));
    #endregion

    #region Member Data
    private readonly ISynchronizeInvoke _synchronizeInvoke;
    #endregion

    #region BackgroundInvokeInterceptor()
    public BackgroundInvokeInterceptor(ISynchronizeInvoke synchronizeInvoke)
    {
      _synchronizeInvoke = synchronizeInvoke;
    }
    #endregion

    #region IInterceptor Members
    public void Intercept(IInvocation invocation)
    {
      BackgroundInvokeAttribute attribute = GetAttribute(invocation.MethodInvocationTarget);
      if (attribute != null)
      {
        if (invocation.Method.ReturnType != typeof(void))
        {
          throw new InvalidOperationException("Can't BackgroundInvoke non-void methods!");
        }
        if (!attribute.OnlyIfInUiThread || !_synchronizeInvoke.InvokeRequired)
        {
          ThreadPool.QueueUserWorkItem(OnCallback, invocation);
        }
        else
        {
          invocation.Proceed();
        }
        return;
      }
      invocation.Proceed();
    }

    private static void OnCallback(object state)
    {
      IInvocation invocation = (IInvocation)state;
      invocation.Proceed();
    }

    private static BackgroundInvokeAttribute GetAttribute(ICustomAttributeProvider method)
    {
      BackgroundInvokeAttribute[] attributes = (BackgroundInvokeAttribute[])method.GetCustomAttributes(typeof(BackgroundInvokeAttribute), true);
      if (attributes.Length != 1)
      {
        return null;
      }
      return attributes[0];
    }
    #endregion
  }
}