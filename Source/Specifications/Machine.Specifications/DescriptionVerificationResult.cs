using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public class ContextVerificationResult
  {
    readonly IEnumerable<SpecificationVerificationResult> _specificationResults;

    public IEnumerable<SpecificationVerificationResult> SpecificationResults
    {
      get { return _specificationResults; }
    }

    public ContextVerificationResult(IEnumerable<SpecificationVerificationResult> specificationResults)
    {
      _specificationResults = specificationResults;
    }
  }
}