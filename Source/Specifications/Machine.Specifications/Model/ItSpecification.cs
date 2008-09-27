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
        return new Result(verificationContext.ThrownException);
      }

      try
      {
        InvokeSpecificationField(verificationContext);
      }
      catch (TargetInvocationException exception)
      {
        return new Result(exception.InnerException);
      }

      return new Result(Status.Passing);
    }
  }
}