using System;
using System.Reflection;
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

namespace Machine.Specifications.Specs
{
    [Subject(typeof(ExceptionResult))]
    public class When_framework_stack_trace_lines_are_filtered
    {
        static ExceptionResult result;

        Because of = () =>
            result = new ExceptionResult(Throw.Exception());

        It should_remove_framework_stack_lines = () =>
            result.StackTrace.ShouldNotContain(" Machine.Specifications.");

        It should_remove_framework_stack_lines_from_the_string_representation = () =>
            result.ToString().ShouldNotContain(" Machine.Specifications.");

        It should_keep_user_stack_lines = () =>
            result.StackTrace.ShouldContain(" SomeProject.Specs.Throw.Exception");
    }

    [Subject(typeof(ExceptionResult))]
    public class When_the_actual_exception_is_wrapped_in_a_TargetInvocationException
    {
        static ExceptionResult result;

        Because of = () =>
            result = new ExceptionResult(new TargetInvocationException(new Exception("inner")));

        It should_only_take_the_inner_exception_into_account = () =>
            result.FullTypeName.ShouldEqual(typeof(Exception).FullName);
    }

    [Subject(typeof(ExceptionResult))]
    public class When_a_TargetInvocationException_is_wrapped
    {
        static ExceptionResult result;

        Because of = () =>
            result = new ExceptionResult(new Exception("outer", new TargetInvocationException(new Exception("inner"))));

        It should_keep_the_exception = () =>
            result.InnerExceptionResult.FullTypeName.ShouldEqual(typeof(TargetInvocationException).FullName);
    }
}
