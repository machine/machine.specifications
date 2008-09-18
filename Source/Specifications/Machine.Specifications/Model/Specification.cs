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
    DelegateField _specificationField;
    DelegateField _whenField;

    public FieldInfo SpecificationField
    {
      get { return _specificationField.Field; }
    }

    public FieldInfo WhenField
    {
      get { return _whenField.Field; }
    }

    public string Name
    {
      get { return _specificationPrefix + _specificationField.Name; }
    }

    public string WhenClause
    {
      get { return _whenField.Name; }
    }

    public bool HasWhenClause
    {
      get { return _whenField != null; }
    }

    protected Specification(FieldInfo itField, FieldInfo whenField) : this("", itField, whenField)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo itField, FieldInfo whenField)
    {
      _specificationPrefix = specificationPrefix;
      _specificationField = new DelegateField(itField);
      if (whenField != null)
      {
        _whenField = new DelegateField(whenField);
      }
    }

    public virtual SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      if (!IsDefined(verificationContext))
      {
        return new SpecificationVerificationResult(Result.NotImplemented);
      }

      InvokeWhenField(verificationContext);

      return InternalVerify(verificationContext);
    }

    void InvokeWhenField(VerificationContext verificationContext)
    {
      if (_whenField != null && _whenField.CanInvokeOn(verificationContext.Instance))
      {
        try
        {
          _whenField.InvokeOn(verificationContext.Instance);
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