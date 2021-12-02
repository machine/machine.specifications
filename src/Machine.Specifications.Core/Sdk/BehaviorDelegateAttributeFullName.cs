namespace Machine.Specifications.Sdk
{
    public class BehaviorDelegateAttributeFullName : DelegateAttributeFullName
    {
        private static readonly string AttributeFullName = typeof(BehaviorDelegateAttribute).FullName;

        public static implicit operator string(BehaviorDelegateAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
