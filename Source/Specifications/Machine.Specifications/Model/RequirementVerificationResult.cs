using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class RequirementVerificationResult
  {
    public bool Passed { get; private set; }
    public Exception Exception { get; private set; }

    public RequirementVerificationResult(Exception exception)
    {
      this.Passed = false;
      this.Exception = exception;
    }

    public RequirementVerificationResult(bool passed)
    {
      this.Passed = passed;
    }
  }
}
