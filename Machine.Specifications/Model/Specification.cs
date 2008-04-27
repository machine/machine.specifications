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

    public Type Type
    {
      get; private set;
    }

    public Specification(Type type, object instance, Action<VerificationContext> contextSetup)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
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
      var requirementResults = VerifyRequirements().ToList();
      return new SpecificationVerificationResult(requirementResults);
    }

    private IEnumerable<RequirementVerificationResult> VerifyRequirements()
    {
      var context = new VerificationContext();
      foreach (Requirement requirement in _requirements)
      {
        _contextSetup(context);
        yield return requirement.Verify(context);
      }
    }
  }
}
