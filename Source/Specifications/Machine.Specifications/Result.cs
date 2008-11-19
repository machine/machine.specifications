using System;
using System.Runtime.Serialization;

namespace Machine.Specifications
{
  public enum Status
  {
    Failing,
    Passing,
    NotImplemented,
    Ignored
  }

  [Serializable]
  public class ExceptionResult
  {
    readonly string _toString;

    public string FullTypeName { get; private set; }
    public string TypeName { get; private set; }
    public string Message { get; private set; }
    public string StackTrace { get; private set; }

    public ExceptionResult(Exception exception)
    {
      FullTypeName = exception.GetType().FullName;
      TypeName = exception.GetType().Name;
      Message = exception.Message;
      _toString = exception.ToString();
      StackTrace = exception.StackTrace;
    }

    public override string ToString()
    {
      return _toString;
    }
  }

  [Serializable]
  public class Result
  {
    readonly Status _status;

    public bool Passed
    {
      get { return _status == Status.Passing; }
    }

    public ExceptionResult Exception { get; private set; }

    public Status Status
    {
      get { return _status; }
    }

    public string ConsoleOut
    {
      get;
      internal set;
    }

    public string ConsoleError
    {
      get;
      internal set;
    }

    private Result(Status status, Exception exception)
    {
      _status = Status.Failing;
      this.Exception = new ExceptionResult(exception);
    }

    private Result(Status status)
    {
      _status = status;
    }

    public static Result Pass()
    {
      return new Result(Status.Passing);
    }

    public static Result Ignored()
    {
      return new Result(Status.Ignored);
    }

    public static Result NotImplemented()
    {
      return new Result(Status.NotImplemented);
    }

    public static Result Failure(Exception exception)
    {
      return new Result(Status.Failing, exception);
    }

    public static Result ContextFailure(Exception exception)
    {
      return new Result(Status.Failing, exception);
    }
  }
}
