using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Specification
  {
    readonly DelegateField _specificationField;
    readonly bool _isIgnored;

    public FieldInfo SpecificationField
    {
      get { return _specificationField.Field; }
    }

    public string Name
    {
      get { return _specificationField.Name; }
    }

    public bool IsIgnored
    {
      get { return _isIgnored; }
    }

    public Specification(FieldInfo itField) 
    {
      _isIgnored = DetermineIfIgnored(itField);
      _specificationField = new DelegateField(itField);
    }

    static bool DetermineIfIgnored(FieldInfo field)
    {
      return field.GetCustomAttributes(typeof(IgnoreAttribute), false).Any();
    }

    public virtual Result Verify(VerificationContext verificationContext)
    {
      if (!IsDefined(verificationContext))
      {
        return Result.NotImplemented();
      }

      return InternalVerify(verificationContext);
    }

    protected virtual Result InternalVerify(VerificationContext verificationContext)
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

    public virtual bool IsDefined(VerificationContext verificationContext)
    {
      return _specificationField.CanInvokeOn(verificationContext.Instance);
    }

    protected virtual void InvokeSpecificationField(VerificationContext verificationContext, params object[] arguments)
    {
      _specificationField.InvokeOn(verificationContext.Instance, arguments);
    }
  }
}