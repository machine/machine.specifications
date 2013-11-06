namespace Machine.Specifications.Sdk
{
    public class BecauseDelegateFullName
    {
        static readonly string becauseDelegateFullName = typeof(Because).FullName;

        public static implicit operator string(BecauseDelegateFullName fullName)
        {
            return becauseDelegateFullName;
        }
    }
}