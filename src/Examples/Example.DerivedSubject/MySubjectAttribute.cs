using System;

using Machine.Specifications;

namespace Example.DerivedSubject
{
    public class MySubjectAttribute : SubjectAttribute
    {
        public MySubjectAttribute(Type subjectType) : base(subjectType)
        {
        }

        public MySubjectAttribute(Type subjectType, string subject) : base(subjectType, subject)
        {
        }

        public MySubjectAttribute(string subject) : base(subject)
        {
        }
    }
}