using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Verifiers
{
  public class ContextVerifier
  {
    public IEnumerable<ContextVerificationResult> VerifyContext(IEnumerable<Model.Context> descriptions)
    {
      foreach (Model.Context description in descriptions)
      {
        yield return description.Verify();
      }
    }
  }
}