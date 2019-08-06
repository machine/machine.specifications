using System;
using Machine.Specifications;

namespace Example.DerivedSubject
{
    public class MySubjectAttribute : SubjectAttribute
    {
        public MySubjectAttribute(Type subjectType, string subject)
            : base(subjectType, subject)
        {
        }
    }
}
