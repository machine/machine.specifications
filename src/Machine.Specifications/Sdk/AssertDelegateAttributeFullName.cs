namespace Machine.Specifications.Sdk
{
  public class AssertDelegateAttributeFullName : DelegateAttributeFullName
  {
    static readonly string assertDelegateAttributeName = typeof(AssertDelegateAttribute).FullName;

    public static implicit operator string(AssertDelegateAttributeFullName fullName)
    {
      return assertDelegateAttributeName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}