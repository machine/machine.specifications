namespace Machine.Specifications.Sdk
{
    public class SubjectAttributeFullName : AttributeFullName
    {
        private static readonly string AttributeFullName = typeof(SubjectAttribute).FullName;

        public static implicit operator string(SubjectAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
