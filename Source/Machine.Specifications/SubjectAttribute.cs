using System;

using Machine.Specifications.Annotations;
using Machine.Specifications.Model;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
  public class SubjectAttribute : Attribute
  {
    readonly Type _subjectType;
    readonly string _subject;

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

    public Subject CreateSubject()
    {
      return new Subject(_subjectType,_subject);
    }
  }
}