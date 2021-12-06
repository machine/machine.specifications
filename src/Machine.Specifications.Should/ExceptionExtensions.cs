using System;

namespace Machine.Specifications
{
    public static class ExceptionExtensions
    {
        public static void ShouldContainErrorMessage(this Exception exception, string expected)
        {
            exception.Message.ShouldContain(expected);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
        {
            var exception = CatchException(method);

            exception.ShouldNotBeNull();
            exception.ShouldBeAssignableTo(exceptionType);

            return exception;
        }

        private static Exception CatchException(Action throwingAction)
        {
            try
            {
                throwingAction();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
    }
}
