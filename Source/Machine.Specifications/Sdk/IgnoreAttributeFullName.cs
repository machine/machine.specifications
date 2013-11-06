namespace Machine.Specifications.Sdk
{
    public class IgnoreAttributeFullName
    {
        static readonly string ignoreAttributeFullName = typeof(Cleanup).FullName;

        public static implicit operator string(IgnoreAttributeFullName fullName)
        {
            return ignoreAttributeFullName;
        }
    }
}