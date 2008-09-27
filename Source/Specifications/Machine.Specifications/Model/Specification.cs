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

    public FieldInfo SpecificationField
    {
      get { return _specificationField.Field; }
    }

    public string Name
    {
      get { return _specificationPrefix + _specificationField.Name; }
    }

    protected Specification(FieldInfo itField) : this("", itField)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo itField)
    {
      _specificationPrefix = specificationPrefix;
      _specificationField = new DelegateField(itField);
    }

    public virtual SpecificationVerificationResult Verify(VerificationContext verificationContext)
    {
      if (!IsDefined(verificationContext))
      {
        return new SpecificationVerificationResult(Status.NotImplemented);
      }

      return InternalVerify(verificationContext);
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