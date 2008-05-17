using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Machine.Specifications;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Machine.Mocks.Specs
{
  public class Interceptor_intercepting_equals 
    : with_interceptor
  {
    Context[] before_each = {()=>
      invocation.Stub(x=>x.Method).Return(typeof(object).GetMethod("Equals"))};

    When Intercept_is_called = ()=>
      interceptor.Intercept(invocation);

    It should_proceed = ()=>
      invocation.AssertWasCalled(x=>x.Proceed());
  }
  
  public class Interceptor_intercepting_to_string 
    : with_interceptor
  {
    Context[] before_each = {()=>
      invocation.Stub(x=>x.Method).Return(typeof(object).GetMethod("ToString"))};

    When Intercept_is_called = ()=>
      interceptor.Intercept(invocation);

    It should_proceed = ()=>
      invocation.AssertWasCalled(x=>x.Proceed());
  }
  
  public class Interceptor_intercepting_non_object_method 
    : with_interceptor
  {
    Context before_each = ()=>
      invocation.Stub(x=>x.Method).Return(typeof(IFoo).GetMethod("Query"));

    When Intercept_is_called = ()=>
      interceptor.Intercept(invocation);

    It should_proceed = ()=>
      invocation.AssertWasNotCalled(x=>x.Proceed());
  }
  
  public class with_interceptor
  {
    protected static MockInterceptor interceptor;
    protected static IInvocation invocation;

    Context before_each = ()=>
    {
      interceptor = new MockInterceptor();
      invocation = MockRepository.GenerateStub<IInvocation>();
    };
    
  }
}
