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
      try
      {
        _verifier();
      }
      catch (Exception err)
      {
        return new RequirementVerificationResult(err);
      }

      return new RequirementVerificationResult(true);
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
      if (verificationContext.ThrownException == null)
      {
        return new RequirementVerificationResult(false);
      }

      try
      {
        _verifier(verificationContext.ThrownException);
      }
      catch (Exception err)
      {
        return new RequirementVerificationResult(err);
      }

      return new RequirementVerificationResult(true);
    }
  }
}
