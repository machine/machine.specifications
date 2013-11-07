namespace Machine.Specifications.Sdk
{
    public class IgnoreAttributeFullName
    {
        static readonly string ignoreAttributeFullName = typeof(IgnoreAttribute).FullName;

        public static implicit operator string(IgnoreAttributeFullName fullName)
        {
            return ignoreAttributeFullName;
        }
    }
}