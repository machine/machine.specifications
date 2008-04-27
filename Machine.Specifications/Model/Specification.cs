using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Specification
  {
    private List<Requirement> _requirements;
    private object _instance;
    private Action<VerificationContext> _contextSetup;
    public string Name { get; private set; }
    public string WhenClause { get; set; }
    public object Instance
    {
      get { return _instance; }
    }

    public IEnumerable<Requirement> Requirements
    {
      get { return _requirements; }
    }

    public Specification(string name, object instance, Action<VerificationContext> contextSetup)
    {
      Name = name;
      _instance = instance;
      _contextSetup = contextSetup;
      _requirements = new List<Requirement>();
    }

    public void AddRequirement(Requirement requirement)
    {
      _requirements.Add(requirement);
    }

    public SpecificationVerificationResult Verify()
    {
      var verificationContext = new VerificationContext();
      var requirementResults = VerifyRequirements(verificationContext).ToList();
      return new SpecificationVerificationResult(requirementResults);
    }

    private IEnumerable<RequirementVerificationResult> VerifyRequirements(VerificationContext verificationContext)
    {
      foreach (Requirement requirement in _requirements)
      {
        _contextSetup(verificationContext);
        yield return requirement.Verify(verificationContext);
      }
    }
  }
}
