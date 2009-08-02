using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal static class ExceptionResultConverter
  {
    internal static TaskException[] ConvertExceptions(ExceptionResult exception, out string message)
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

        exceptions.Add(new TaskException(new SpecException(current)));
        current = current.InnerExceptionResult;
      }

      return exceptions.ToArray();
    }
  }

  [Serializable]
  public class SpecException : Exception
  {
    readonly ExceptionResult _exceptionResult;

    public SpecException(ExceptionResult exceptionResult) : base(exceptionResult.Message)
    {
      _exceptionResult = exceptionResult;
    }

    protected SpecException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string StackTrace
    {
      get
      {
        return _exceptionResult.StackTrace;
      }
    }

    public override string Message
    {
      get
      {
        return Environment.NewLine + _exceptionResult.FullTypeName + ": " + _exceptionResult.Message;
      }
    }

    public override string ToString()
    {
      return _exceptionResult.ToString();
    }
  }
}