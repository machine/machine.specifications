using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class SpecificationVerificationResult
  {
    public bool Passed { get; private set; }
    public Exception Exception { get; private set; }

    public SpecificationVerificationResult(Exception exception)
    {
      this.Passed = false;
      this.Exception = exception;
    }

    public SpecificationVerificationResult(bool passed)
    {
      this.Passed = passed;
    }
  }
}
