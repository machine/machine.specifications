using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Verifiers
{
  public class SpecificationVerifier
  {
    public IEnumerable<SpecificationVerificationResult> VerifySpecifications(IEnumerable<Specification> specifications)
    {
      foreach (Specification specification in specifications)
      {
        yield return specification.Verify();
      }
    }
  }
}
