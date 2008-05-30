using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public enum Result
  {
    Failed,
    Passed,
    Unknown
  }

  public class SpecificationVerificationResult
  {
    readonly Result _result;

    public bool Passed
    {
      get { return _result == Result.Passed; }
    }

    public Exception Exception { get; private set; }

    public Result Result
    {
      get { return _result; }
    }

    public SpecificationVerificationResult(Exception exception)
    {
      _result = Result.Failed;
      this.Exception = exception;
    }

    public SpecificationVerificationResult()
    {
      _result = Result.Passed;
    }

    public SpecificationVerificationResult(Result result)
    {
      this._result = result;
    }
  }
}