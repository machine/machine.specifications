using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class SpecificationVerificationResult
  {
    private IEnumerable<RequirementVerificationResult> _requirementResults;

    public IEnumerable<RequirementVerificationResult> RequirementResults
    {
      get { return _requirementResults; }
    }

    public SpecificationVerificationResult(IEnumerable<RequirementVerificationResult> requirementResults)
    {
      _requirementResults = requirementResults;
    }
  }
}
