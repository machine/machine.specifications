using System;
using JetBrains.Annotations;
using Machine.Specifications.Model;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public class SubjectAttribute : Attribute
    {
        private readonly Type subjectType;

        private readonly string subject;

        public SubjectAttribute(Type subjectType)
        {
            this.subjectType = subjectType;
        }

        public SubjectAttribute(Type subjectType, string subject)
        {
            this.subjectType = subjectType;
            this.subject = subject;
        }

        public SubjectAttribute(string subject)
        {
            this.subject = subject;
        }

        public Subject CreateSubject()
        {
            return new Subject(subjectType, subject);
        }
    }
}
