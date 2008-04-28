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
    private IEnumerable<Before> _beforeEachs;
    private IEnumerable<Before> _beforeAlls;
    private IEnumerable<After> _afterEachs;
    private IEnumerable<After> _afterAlls;
    private When _when;
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

    public Specification(Type type, object instance, IEnumerable<Before> beforeEachs, IEnumerable<Before> beforeAlls, IEnumerable<After> afterEachs, IEnumerable<After> afterAlls, When when)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
      _instance = instance;
      _when = when;
      _afterAlls = afterAlls;
      _afterEachs = afterEachs;
      _beforeAlls = beforeAlls;
      _beforeEachs = beforeEachs;
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
      _beforeAlls.InvokeAll();
      var results = ExecuteRequirements();
      _afterAlls.InvokeAll();

      return results;
    }

    private IEnumerable<RequirementVerificationResult> ExecuteRequirements()
    {
      var context = new VerificationContext();
      var results = new List<RequirementVerificationResult>();
      foreach (Requirement requirement in _requirements)
      {
        _beforeEachs.InvokeAll();
        if (_when != null)
        {
          try
          {
            _when();
          }
          catch (Exception exception)
          {
            context.ThrownException = exception;
          }
        }
        results.Add(requirement.Verify(context));
        _afterEachs.InvokeAll();
      }

      return results;
    }
  }
}
