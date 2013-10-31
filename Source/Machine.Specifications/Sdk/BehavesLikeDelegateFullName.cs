namespace Machine.Specifications.Sdk
{
    public class BehavesLikeDelegateFullName
    {
        static readonly string behaveLikeAttributeFullname = typeof(Behaves_like<>).FullName;

        public static implicit operator string(BehavesLikeDelegateFullName fullName)
        {
            return behaveLikeAttributeFullname;
        }
    }

    public class EstablishDelegateFullName
    {
        static readonly string establishDelegateFullName = typeof(Establish).FullName;

        public static implicit operator string(EstablishDelegateFullName fullName)
        {
            return establishDelegateFullName;
        }
    }

    public class BecauseDelegateFullName
    {
        static readonly string becauseDelegateFullName = typeof(Because).FullName;

        public static implicit operator string(BecauseDelegateFullName fullName)
        {
            return becauseDelegateFullName;
        }
    }

    public class CleanupDelegateFullName
    {
        static readonly string becauseDelegateFullName = typeof(Cleanup).FullName;

        public static implicit operator string(CleanupDelegateFullName fullName)
        {
            return becauseDelegateFullName;
        }
    }

    public class IgnoreAttributeFullName
    {
        static readonly string ignoreAttributeFullName = typeof(Cleanup).FullName;

        public static implicit operator string(IgnoreAttributeFullName fullName)
        {
            return ignoreAttributeFullName;
        }
    }
}