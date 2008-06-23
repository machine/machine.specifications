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
  [Concerning(typeof(MockInterceptor))]
  public class When_intercepting_invocation_of_Equals
    : with_interceptor
  {
    Because of = () =>
      InterceptMethod(typeof(object).GetMethod("Equals", new[] { typeof(object) }));

    It should_proceed = () =>
      invocation.AssertWasCalled(x => x.Proceed());
  }

  [Concerning(typeof(MockInterceptor))]
  public class When_intercepting_invocation_of_ToString
    : with_interceptor
  {
    Because of = () =>
      InterceptMethod(typeof(object).GetMethod("ToString"));

    It should_proceed = () =>
      invocation.AssertWasCalled(x => x.Proceed());
  }

  [Concerning(typeof(MockInterceptor))]
  public class When_intercepting_invocation_of_GetHashCode
    : with_interceptor
  {
    Because of = () =>
      InterceptMethod(typeof(object).GetMethod("GetHashCode"));

    It should_proceed = () =>
      invocation.AssertWasCalled(x => x.Proceed());
  }

  [Concerning(typeof(MockInterceptor))]
  public class When_intercepting_invocation_of_GetType
    : with_interceptor
  {
    Because of = () =>
      InterceptMethod(typeof(object).GetMethod("GetType"));

    It should_proceed = () =>
      invocation.AssertWasCalled(x => x.Proceed());
  }

  [Concerning(typeof(MockInterceptor))]
  public class When_intercepting_invocation_of_non_object_method
    : with_interceptor
  {
    Because Intercepting_non_object_method = () =>
      InterceptMethod(typeof(IFoo).GetMethod("Query"));

    It should_not_proceed = () =>
      invocation.AssertWasNotCalled(x => x.Proceed());
  }

  public class with_interceptor
  {
    protected static MockInterceptor interceptor;
    protected static IInvocation invocation;

    Establish context = () =>
    {
      interceptor = new MockInterceptor();
      invocation = MockRepository.GenerateStub<IInvocation>();
    };

    protected static void InterceptMethod(MethodInfo methodInfo)
    {
      invocation.Stub(x => x.Method).Return(methodInfo);
      interceptor.Intercept(invocation);
    }
  }
}