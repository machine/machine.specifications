namespace Machine.Specifications.Sdk
{
    public class EstablishDelegateFullName
    {
        static readonly string establishDelegateFullName = typeof(Establish).FullName;

        public static implicit operator string(EstablishDelegateFullName fullName)
        {
            return establishDelegateFullName;
        }
    }
}