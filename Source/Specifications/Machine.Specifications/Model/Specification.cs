using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public abstract class Specification
  {
    readonly string _specificationPrefix;
    readonly DelegateField _specificationField;
    readonly DelegateField _becauseField;

    public FieldInfo SpecificationField
    {
      get { return _specificationField.Field; }
    }

    public FieldInfo BecauseField
    {
      get { return _becauseField.Field; }
    }

    public string Name
    {
      get { return _specificationPrefix + _specificationField.Name; }
    }

    public string BecauseClause
    {
      get { return _becauseField.Name; }
    }

    public bool HasBecauseClause
    {
      get { return _becauseField != null; }
    }

    protected Specification(FieldInfo itField, FieldInfo becauseField) : this("", itField, becauseField)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo itField, FieldInfo becauseField)
    {
      _specificationPrefix = specificationPrefix;
      _specificationField = new DelegateField(itField);
      if (becauseField != null)
      {
        _becauseField = new DelegateField(becauseField);
      }
    }

    public virtual SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      if (!IsDefined(verificationContext))
      {
        return new SpecificationVerificationResult(Status.NotImplemented);
      }

      InvokeBecauseField(verificationContext);

      return InternalVerify(verificationContext);
    }

    void InvokeBecauseField(VerificationContext verificationContext)
    {
      if (_becauseField != null && _becauseField.CanInvokeOn(verificationContext.Instance))
      {
        try
        {
          _becauseField.InvokeOn(verificationContext.Instance);
        }
        catch (TargetInvocationException exception)
        {
          verificationContext.ThrownException = exception.InnerException;
        }
      }
    }

    protected abstract SpecificationVerificationResult InternalVerify(VerificationContext verificationContext);

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