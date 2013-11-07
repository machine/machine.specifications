namespace Machine.Specifications.Sdk
{
    public class SubjectAttributeFullName
    {
        static readonly string subjectAttributeFullName = typeof(SubjectAttribute).FullName;

        public static implicit operator string(SubjectAttributeFullName fullName)
        {
            return subjectAttributeFullName;
        }
    }
}