namespace Machine.Specifications.Sdk
{
    public class ItDelegateFullName
    {
        static readonly string itDelegateFullName = typeof(It).FullName;

        public static implicit operator string(ItDelegateFullName fullName)
        {
            return itDelegateFullName;
        }
    }
}