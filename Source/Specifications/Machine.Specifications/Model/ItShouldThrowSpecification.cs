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
        try
        {
          throw new SpecificationException("Expected a thrown exception but didn't get one.");
        }
        catch (Exception exception)
        {
          return new SpecificationVerificationResult(exception);
        }
      }

      try
      {
        InvokeSpecificationField(verificationContext, verificationContext.ThrownException);
      }
      catch (TargetInvocationException exception)
      {
        return new SpecificationVerificationResult(exception.InnerException);
      }

      return new SpecificationVerificationResult(Result.Passed);
    }
  }
}