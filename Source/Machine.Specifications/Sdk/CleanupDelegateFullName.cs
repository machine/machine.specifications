namespace Machine.Specifications.Sdk
{
    public class CleanupDelegateFullName
    {
        static readonly string becauseDelegateFullName = typeof(Cleanup).FullName;

        public static implicit operator string(CleanupDelegateFullName fullName)
        {
            return becauseDelegateFullName;
        }
    }
}