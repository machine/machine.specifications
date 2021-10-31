namespace Machine.Specifications.Sdk
{
    public class SetupDelegateAttributeFullName : DelegateAttributeFullName
    {
        private static readonly string AttributeFullName = typeof(SetupDelegateAttribute).FullName;

        public static implicit operator string(SetupDelegateAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
