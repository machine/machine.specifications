namespace Machine.Specifications.Sdk
{
    public class TagsAttributeFullName : AttributeFullName
    {
        static readonly string AttributeFullName = typeof(TagsAttribute).FullName;

        public static implicit operator string(TagsAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
