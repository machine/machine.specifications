using System;
using System.Reflection;

namespace Machine.Specifications.Model
{
  public class ItSpecification : Specification
  {
    public ItSpecification(FieldInfo itField) : base(itField)
    {
    }

    protected override SpecificationVerificationResult InternalVerify(VerificationContext verificationContext)
    {
      if (verificationContext.ThrownException != null)
      {
        return new SpecificationVerificationResult(verificationContext.ThrownException);
      }

      try
      {
        InvokeSpecificationField(verificationContext);
      }
      catch (TargetInvocationException exception)
      {
        return new SpecificationVerificationResult(exception.InnerException);
      }

      return new SpecificationVerificationResult(Status.Passing);
    }
  }
}