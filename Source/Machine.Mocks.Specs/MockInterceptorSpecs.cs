using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Machine.Mocks.Implementation;
using Machine.Specifications;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Machine.Mocks.InterceptorSpecs
{
  public class Interceptor_intercepting_methods
    : with_interceptor
  {
    When Intercepting_Equals = () =>
      InterceptMethod(typeof(object).GetMethod("Equals", new[] { typeof(object) }));

    Or_when Intercepting_ToString = () =>
      InterceptMethod(typeof(object).GetMethod("ToString"));

    Or_when Intercepting_GetHashCode = () =>
      InterceptMethod(typeof(object).GetMethod("GetHashCode"));

    Or_when Intercepting_GetType = () =>
      InterceptMethod(typeof(object).GetMethod("GetType"));

    It should_proceed = () =>
      invocation.AssertWasCalled(x => x.Proceed());




    When Intercepting_non_object_method = () =>
      InterceptMethod(typeof(IFoo).GetMethod("Query"));

    It should_not_proceed = () =>
      invocation.AssertWasNotCalled(x => x.Proceed());

    static void InterceptMethod(MethodInfo methodInfo)
    {
      invocation.Stub(x => x.Method).Return(methodInfo);
      interceptor.Intercept(invocation);
    }
  }

  public class with_interceptor
  {
    protected static MockInterceptor interceptor;
    protected static IInvocation invocation;

    Context before_each = () =>
    {
      interceptor = new MockInterceptor();
      invocation = MockRepository.GenerateStub<IInvocation>();
    };

  }
}