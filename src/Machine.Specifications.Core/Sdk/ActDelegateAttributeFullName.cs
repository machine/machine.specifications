namespace Machine.Specifications.Sdk
{
    public class ActDelegateAttributeFullName : DelegateAttributeFullName
    {
        private static readonly string AttributeFullName = typeof(ActDelegateAttribute).FullName;

        public static implicit operator string(ActDelegateAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
