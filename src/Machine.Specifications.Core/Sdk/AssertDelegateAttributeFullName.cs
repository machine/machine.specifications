namespace Machine.Specifications.Sdk
{
    public class AssertDelegateAttributeFullName : DelegateAttributeFullName
    {
        private static readonly string AttributeFullName = typeof(AssertDelegateAttribute).FullName;

        public static implicit operator string(AssertDelegateAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
