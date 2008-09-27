using System;
using System.Reflection;

namespace Machine.Specifications.Model
{
  public class ItSpecification : Specification
  {
    public ItSpecification(FieldInfo itField) : base(itField)
    {
    }

    protected override Result InternalVerify(VerificationContext verificationContext)
    {
      if (verificationContext.ThrownException != null)
      {
        return Result.Failure(verificationContext.ThrownException);
      }

      try
      {
        InvokeSpecificationField(verificationContext);
      }
      catch (TargetInvocationException exception)
      {
        return Result.Failure(exception.InnerException);
      }

      return Result.Pass();
    }
  }
}