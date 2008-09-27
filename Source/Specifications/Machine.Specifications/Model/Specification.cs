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
    readonly bool _isIgnored;

    public FieldInfo SpecificationField
    {
      get { return _specificationField.Field; }
    }

    public string Name
    {
      get { return _specificationPrefix + _specificationField.Name; }
    }

    public bool IsIgnored
    {
      get { return _isIgnored; }
    }

    protected Specification(FieldInfo itField) : this("", itField)
    {
    }

    protected Specification(string specificationPrefix, FieldInfo itField)
    {
      _isIgnored = DetermineIfIgnored(itField);
      _specificationPrefix = specificationPrefix;
      _specificationField = new DelegateField(itField);
    }

    static bool DetermineIfIgnored(FieldInfo field)
    {
      return field.GetCustomAttributes(typeof(IgnoreAttribute), false).Any();
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