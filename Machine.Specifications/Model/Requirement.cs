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

    public FieldInfo Field
    {
      get; private set;
    }

    protected Requirement(FieldInfo fieldInfo)
    {
      ItClause = fieldInfo.Name.ReplaceUnderscores();
      Field = fieldInfo;
    }

    public abstract RequirementVerificationResult Verify(VerificationContext verificationContext);
  }

  public class ItRequirement : Requirement
  {
    private It _verifier;

    public ItRequirement(FieldInfo fieldInfo, It verifier) : base(fieldInfo)
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

    public ItShouldThrowRequirement(FieldInfo fieldInfo, It_should_throw verifier) : base(fieldInfo)
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
