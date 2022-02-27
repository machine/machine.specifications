using System;

namespace Machine.Specifications
{
    public static class ExceptionExtensions
    {
        public static void ShouldContainErrorMessage(this Exception exception, string expected)
        {
            exception.Message.ShouldContain(expected);
        }
    }
}
