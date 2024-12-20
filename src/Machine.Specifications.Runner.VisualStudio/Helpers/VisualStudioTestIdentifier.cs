using System;
using System.Globalization;

namespace Machine.Specifications.Runner.VisualStudio.Helpers
{
#if NETFRAMEWORK
    [Serializable]
#endif
    public class VisualStudioTestIdentifier
    {
        public VisualStudioTestIdentifier()
        {
        }

        public VisualStudioTestIdentifier(string containerTypeFullName, string fieldName)
            : this(string.Format(CultureInfo.InvariantCulture, "{0}::{1}", containerTypeFullName, fieldName))
        {
        }

        public VisualStudioTestIdentifier(string fullyQualifiedName)
        {
            FullyQualifiedName = fullyQualifiedName;
        }

        public string FullyQualifiedName { get; private set; }

        public string FieldName
        {
            get
            {
                return FullyQualifiedName.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        public string ContainerTypeFullName
        {
            get
            {
                return FullyQualifiedName.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is VisualStudioTestIdentifier test)
            {
                return FullyQualifiedName.Equals(test.FullyQualifiedName, StringComparison.Ordinal);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return FullyQualifiedName.GetHashCode();
        }
    }
}
