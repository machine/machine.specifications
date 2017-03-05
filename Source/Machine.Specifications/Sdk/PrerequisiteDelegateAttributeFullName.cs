namespace Machine.Specifications.Sdk
{
    public class PrerequisiteDelegateAttributeFullName : DelegateAttributeFullName
    {
        static readonly string assertDelegateAttributeName = typeof(PrerequisiteDelegateAttribute).FullName;

        public static implicit operator string(PrerequisiteDelegateAttributeFullName fullName)
        {
            return assertDelegateAttributeName;
        }

        public override string FullName
        {
            get { return this; }
        }
    }
}