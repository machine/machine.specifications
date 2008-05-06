using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class DescriptionVerificationResult
  {
    private readonly IEnumerable<SpecificationVerificationResult> _specificationResults;

    public IEnumerable<SpecificationVerificationResult> SpecificationResults
    {
      get { return _specificationResults; }
    }

    public DescriptionVerificationResult(IEnumerable<SpecificationVerificationResult> specificationResults)
    {
      _specificationResults = specificationResults;
    }
  }
}
