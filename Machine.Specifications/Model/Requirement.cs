using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public abstract class Requirement
  {
    public string ItClause { get; private set; }

    public Requirement(string itClause)
    {
      ItClause = itClause;
    }

    public abstract RequirementVerificationResult Verify(VerificationContext verificationContext);
  }

  public class ItRequirement : Requirement
  {
    private It _verifier;

    public ItRequirement(string itClause, It verifier) : base(itClause)
    {
      _verifier = verifier;
    }

    public override RequirementVerificationResult Verify(VerificationContext verificationContext)
    {
      _verifier();

      return new RequirementVerificationResult();
    }
  }

  public class ItShouldThrowRequirement : Requirement
  {
    private It_should_throw _verifier;

    public ItShouldThrowRequirement(string itClause, It_should_throw verifier) : base(itClause)
    {
      _verifier = verifier;
    }

    public override RequirementVerificationResult Verify(VerificationContext verificationContext)
    {
      _verifier(verificationContext.ThrownException);

      return new RequirementVerificationResult();
    }
  }
}
