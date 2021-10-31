namespace Machine.Specifications.Sdk
{
    public class CleanupDelegateAttributeFullName : DelegateAttributeFullName
    {
        private static readonly string AttributeFullName = typeof(CleanupDelegateAttribute).FullName;

        public static implicit operator string(CleanupDelegateAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
