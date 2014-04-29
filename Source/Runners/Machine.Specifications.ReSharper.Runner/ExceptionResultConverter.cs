using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ReSharperRunner
{
    using System.Collections.Generic;
    using System.Reflection;

    using JetBrains.ReSharper.TaskRunnerFramework;

    public static class ExceptionResultConverter
    {
        public static TaskException[] ConvertExceptions(ExceptionResult exception, out string message)
        {
            ExceptionResult current;
            if ((exception.FullTypeName == typeof(TargetInvocationException).FullName) && (exception.InnerExceptionResult != null))
            {
                current = exception.InnerExceptionResult;
            }
            else
            {
                current = exception;
            }

            message = null;
            var exceptions = new List<TaskException>();
            while (current != null)
            {
                if (message == null)
                {
                    message = string.Format("{0}: {1}", current.FullTypeName, current.Message);
                }

                exceptions.Add(new TaskException(current.FullTypeName, current.Message, current.StackTrace));
                current = current.InnerExceptionResult;
            }

            return exceptions.ToArray();
        }
    }
}