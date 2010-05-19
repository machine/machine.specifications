using System;

using Machine.Specifications.Annotations;
using Machine.Specifications.Utility;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  public class SubjectAttribute : Attribute
  {
    readonly Type _subjectType;
    readonly string _subject;

    public Type SubjectType
    {
      get { return _subjectType; }
    }

    public string SubjectText
    {
      get { return _subject; }
    }

    public SubjectAttribute(Type subjectType)
    {
      this._subjectType = subjectType;
    }

    public SubjectAttribute(Type subjectType, string subject)
    {
      _subjectType = subjectType;
      _subject = subject;
    }

    public SubjectAttribute(string subject)
    {
      _subject = subject;
    }
  }
}
