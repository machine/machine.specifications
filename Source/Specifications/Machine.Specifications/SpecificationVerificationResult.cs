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
  public class SpecificationVerificationResult
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

    public SpecificationVerificationResult(Exception exception)
    {
      _status = Status.Failing;
      this.Exception = exception;
    }

    public SpecificationVerificationResult()
    {
      _status = Status.Passing;
    }

    public SpecificationVerificationResult(Status status)
    {
      _status = status;
    }

    public static SpecificationVerificationResult Ignored
    {
      get { return new SpecificationVerificationResult(Status.Ignored); }
    }
  }
}