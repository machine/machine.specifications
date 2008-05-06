using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public abstract class Specification
  {
    public string ItClause { get; private set; }

    public FieldInfo Field
    {
      get; private set;
    }

    protected Specification(FieldInfo fieldInfo) : this("", fieldInfo)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo fieldInfo)
    {
      ItClause = specificationPrefix + fieldInfo.Name.ReplaceUnderscores();
      Field = fieldInfo;
    }

    public abstract SpecificationVerificationResult Verify(VerificationContext verificationContext);
  }

  public class ItSpecification : Specification
  {
    private It _verifier;

    public ItSpecification(FieldInfo fieldInfo, It verifier) : base(fieldInfo)
    {
      _verifier = verifier;
    }

    public override SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      try
      {
        _verifier();
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }

      return new SpecificationVerificationResult(true);
    }
  }

  public class ItShouldThrowSpecification : Specification
  {
    private It_should_throw _verifier;

    public ItShouldThrowSpecification(FieldInfo fieldInfo, It_should_throw verifier) : base("should throw ", fieldInfo)
    {
      _verifier = verifier;
    }

    public override SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      if (verificationContext.ThrownException == null)
      {
        return new SpecificationVerificationResult(false);
      }

      try
      {
        _verifier(verificationContext.ThrownException);
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }

      return new SpecificationVerificationResult(true);
    }
  }
}
