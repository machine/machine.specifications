using System;
using System.Collections.Generic;
using System.ComponentModel;

using Castle.Core.Interceptor;

namespace Machine.Core.Aspects
{
  public class SynchronizeInvokeInterceptor : IInterceptor
  {
    private static readonly object[] EMPTY_ARGS = new object[0];

    private readonly ISynchronizeInvoke _synchronizer;
    private delegate void Invoker();

    public SynchronizeInvokeInterceptor(ISynchronizeInvoke synchronizer)
    {
      _synchronizer = synchronizer;
    }

    public void Intercept(IInvocation invocation)
    {
      if (invocation.MethodInvocationTarget.GetCustomAttributes(typeof(SynchronizeInvokeIgnoreAttribute), true).Length > 0)
      {
        invocation.Proceed();
        return;
      }
      if (_synchronizer.InvokeRequired)
      {
        InvocationInvoker invoker = new InvocationInvoker(invocation);
        if (invocation.Method.ReturnType == typeof(void))
        {
          _synchronizer.BeginInvoke(new Invoker(invoker.Invoke), EMPTY_ARGS);
        }
        else
        {
          _synchronizer.Invoke(new Invoker(invoker.Invoke), EMPTY_ARGS);
        }
      }
      else
      {
        invocation.Proceed();
      }
    }
    class InvocationInvoker
    {
      private readonly IInvocation _invocation;

      public InvocationInvoker(IInvocation invocation)
      {
        _invocation = invocation;
      }

      public void Invoke()
      {
        _invocation.Proceed();
      }
    }
  }
}