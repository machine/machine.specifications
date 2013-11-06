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
}