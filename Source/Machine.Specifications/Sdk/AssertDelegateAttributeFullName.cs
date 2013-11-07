namespace Machine.Specifications.Sdk
{
  public class AssertDelegateAttributeFullName
  {
    static readonly string assertDelegateAttributeName = typeof(AssertDelegateAttribute).FullName;

    public static implicit operator string(AssertDelegateAttributeFullName fullName)
    {
      return assertDelegateAttributeName;
    }
  }
}