using System;

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
  public class Result
  {
    readonly Status _status;

    public bool Passed
    {
      get { return _status == Status.Passing; }
    }

    public Exception Exception { get; private set; }

    public Status Status
    {
      get { return _status; }
    }

    public Result(Exception exception)
    {
      _status = Status.Failing;
      this.Exception = exception;
    }

    public Result()
    {
      _status = Status.Passing;
    }

    public Result(Status status)
    {
      _status = status;
    }

    public static Result Ignored
    {
      get { return new Result(Status.Ignored); }
    }
  }
}