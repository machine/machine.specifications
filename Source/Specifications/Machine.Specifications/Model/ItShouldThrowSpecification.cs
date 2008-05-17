using System;
using System.Reflection;

namespace Machine.Specifications.Model
{
  public class ItShouldThrowSpecification : Specification
  {
    public ItShouldThrowSpecification(FieldInfo itField, FieldInfo whenField) : base("should throw ", itField, whenField)
    {
    }

    protected override SpecificationVerificationResult InternalVerify(VerificationContext verificationContext)
    {
      if (verificationContext.ThrownException == null)
      {
        return new SpecificationVerificationResult(Result.Failed);
      }

      try
      {
        InvokeSpecificationField(verificationContext, verificationContext.ThrownException);
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }

      return new SpecificationVerificationResult(Result.Passed);
    }
  }
}