using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class DescriptionVerificationResult
  {
    private IEnumerable<RequirementVerificationResult> _requirementResults;

    public IEnumerable<RequirementVerificationResult> RequirementResults
    {
      get { return _requirementResults; }
    }

    public DescriptionVerificationResult(IEnumerable<RequirementVerificationResult> requirementResults)
    {
      _requirementResults = requirementResults;
    }
  }
}
