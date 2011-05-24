using System;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
    internal abstract class FieldElement : Element
    {
        readonly string _fieldName;

        protected FieldElement(MSpecUnitTestProvider provider,
                               Element parent,
                               ProjectModelElementEnvoy projectEnvoy,
                               string declaringTypeName,
                               string fieldName,
                               bool isIgnored)
            : base(provider, parent, projectEnvoy, declaringTypeName, isIgnored || parent.Explicit)
        {
            _fieldName = fieldName;
        }

        public override string ShortName
        {
            get { return FieldName; }
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public override string GetPresentation()
        {
            return String.Format("{0}{1}{2}",
                                 GetTitlePrefix(),
                                 String.IsNullOrEmpty(GetTitlePrefix()) ? String.Empty : " ",
                                 FieldName.ToFormat());
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType == null)
            {
                return null;
            }

            return declaredType.EnumerateMembers(FieldName, false)
              .Where(member => member as IField != null)
              .FirstOrDefault();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                FieldElement other = (FieldElement)obj;
                return Equals(Parent, other.Parent) && FieldName == other.FieldName;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 29 * result + Parent.GetHashCode();
            result = 29 * result + FieldName.GetHashCode();
            return result;
        }
    }
}
