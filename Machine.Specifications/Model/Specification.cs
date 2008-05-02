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
    private IEnumerable<Context> _beforeEachs;
    private IEnumerable<Context> _beforeAlls;
    private IEnumerable<Context> _afterEachs;
    private IEnumerable<Context> _afterAlls;
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

    public Specification(Type type, object instance, IEnumerable<Context> beforeEachs, IEnumerable<Context> beforeAlls, IEnumerable<Context> afterEachs, IEnumerable<Context> afterAlls, When when)
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
      var requirementResults = VerifyRequirements();
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
      var results = new List<RequirementVerificationResult>();
      foreach (Requirement requirement in _requirements)
      {
        var result = VerifyRequirement(requirement);
        results.Add(result);
      }

      return results;
    }

    public RequirementVerificationResult VerifyRequirement(Requirement requirement)
    {
      VerificationContext context = new VerificationContext();
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
      var result = requirement.Verify(context);
      _afterEachs.InvokeAll();

      return result;
    }

    public void RunContextBeforeAll()
    {
      _beforeAlls.InvokeAll();
    }

    public void RunContextAfterAll()
    {
      _afterAlls.InvokeAll();
    }
  }
}
