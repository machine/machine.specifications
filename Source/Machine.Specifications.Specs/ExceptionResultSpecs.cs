using System;

using Machine.Specifications;

using SomeProject.Specs;


namespace SomeProject.Specs
{
  static class Throw
  {
    public static Exception Exception()
    {
      try
      {
        1.ShouldEqual(2);
      }
      catch (Exception ex)
      {
        return ex;
      }

      return null;
    }
  }
}

#if CLEAN_EXCEPTION_STACK_TRACE
namespace Machine.Specifications.Specs
{
  [Subject(typeof(ExceptionResult))]
  public class When_framework_stack_trace_lines_are_filtered
  {
    static ExceptionResult Result;

    Because of = () => { Result = new ExceptionResult(Throw.Exception()); };

    It should_remove_framework_stack_lines =
      () => Result.StackTrace.ShouldNotContain(" Machine.Specifications.");

    It should_keep_user_stack_lines =
      () => Result.StackTrace.ShouldContain(" SomeProject.Specs.Throw.Exception");
  }
}
#endif