using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public abstract class Specification
  {
    public string Name { get; private set; }

    public FieldInfo Field
    {
      get; private set;
    }

    public abstract bool IsDefined { get; }

    protected Specification(FieldInfo fieldInfo) : this("", fieldInfo)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo fieldInfo)
    {
      Name = specificationPrefix + fieldInfo.Name.ReplaceUnderscores();
      Field = fieldInfo;
    }

    public virtual SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      if (!IsDefined)
      {
        return new SpecificationVerificationResult(Result.Unknown);
      }

      return InternalVerify(verificationContext);
    }

    protected abstract SpecificationVerificationResult InternalVerify(VerificationContext verificationContext);
  }

  public class ItSpecification : Specification
  {
    private It _verifier;
    
    public override bool IsDefined
    {
      get { return _verifier != null; }
    }

    public ItSpecification(FieldInfo fieldInfo, It verifier) : base(fieldInfo)
    {
      _verifier = verifier;
    }

    protected override SpecificationVerificationResult InternalVerify(VerificationContext verificationContext)
    {
      try
      {
        _verifier();
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }

      return new SpecificationVerificationResult(Result.Passed);
    }
  }

  public class ItShouldThrowSpecification : Specification
  {
    private It_should_throw _verifier;

    public override bool IsDefined
    {
      get { return _verifier != null; }
    }

    public ItShouldThrowSpecification(FieldInfo fieldInfo, It_should_throw verifier) : base("should throw ", fieldInfo)
    {
      _verifier = verifier;
    }

    protected override SpecificationVerificationResult InternalVerify(VerificationContext verificationContext)
    {
      if (verificationContext.ThrownException == null)
      {
        return new SpecificationVerificationResult(Result.Failed);
      }

      try
      {
        _verifier(verificationContext.ThrownException);
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }

      return new SpecificationVerificationResult(Result.Passed);
    }
  }
}
