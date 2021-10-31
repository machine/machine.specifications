namespace Machine.Specifications.Sdk
{
    public class IgnoreAttributeFullName : AttributeFullName
    {
        private static readonly string AttributeFullName = typeof(IgnoreAttribute).FullName;

        public static implicit operator string(IgnoreAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
