using System;
using System.Reflection;

using FluentAssertions;

using Machine.Specifications;

#if CLEAN_EXCEPTION_STACK_TRACE
using SomeProject.Specs;

namespace SomeProject.Specs
{
  static class Throw
  {
    public static Exception Exception()
    {
      try
      {
        1.Should().Be(2);
      }
      catch (Exception ex)
      {
        return ex;
      }

      return null;
    }
  }
}

namespace Machine.Specifications.Specs
{
  [Subject(typeof(ExceptionResult))]
  public class When_framework_stack_trace_lines_are_filtered
  {
    static ExceptionResult Result;

    Because of = () => { Result = new ExceptionResult(Throw.Exception()); };

    It should_remove_framework_stack_lines =
      () => Result.StackTrace.Should().NotContain(" Machine.Specifications.");

    It should_remove_framework_stack_lines_from_the_string_representation =
      () => Result.ToString().Should().NotContain(" Machine.Specifications.");

    It should_keep_user_stack_lines =
      () => Result.StackTrace.Should().Contain(" SomeProject.Specs.Throw.Exception");
  }

  [Subject(typeof(ExceptionResult))]
  public class When_the_actual_exception_is_wrapped_in_a_TargetInvocationException
  {
    static ExceptionResult Result;

    Because of = () => { Result = new ExceptionResult(new TargetInvocationException(new Exception("inner"))); };

    It should_only_take_the_inner_exception_into_account =
      () => Result.FullTypeName.Should().Be(typeof(Exception).FullName);
  }

  [Subject(typeof(ExceptionResult))]
  public class When_a_TargetInvocationException_is_wrapped
  {
    static ExceptionResult Result;

    Because of = () => { Result = new ExceptionResult(new Exception("outer", new TargetInvocationException(new Exception("inner")))); };

    It should_keep_the_exception =
      () => Result.InnerExceptionResult.FullTypeName.Should().Be(typeof(TargetInvocationException).FullName);
  }
}
#endif