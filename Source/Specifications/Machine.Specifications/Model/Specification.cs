using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Specification
  {
    readonly string _name;
    readonly It _it;
    readonly bool _isIgnored;
    readonly FieldInfo _fieldInfo;

    public FieldInfo FieldInfo
    {
      get { return _fieldInfo; }
    }

    public string Name
    {
      get { return _name; }
    }

    public bool IsIgnored
    {
      get { return _isIgnored; }
    }

    public Specification(string name, It it, bool isIgnored, FieldInfo fieldInfo)
    {
      _name = name;
      _it = it;
      _isIgnored = isIgnored;
      _fieldInfo = fieldInfo;
    }

    public virtual Result Verify(VerificationContext verificationContext)
    {
      if (!IsDefined)
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
        InvokeSpecificationField();
      }
      catch (Exception exception)
      {
        return Result.Failure(exception);
      }

      return Result.Pass();
    }

    public bool IsDefined
    {
      get { return _it != null; }
    }

    protected virtual void InvokeSpecificationField()
    {
      _it.Invoke();
    }
  }
}