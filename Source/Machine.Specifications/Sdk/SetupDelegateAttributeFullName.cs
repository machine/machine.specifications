namespace Machine.Specifications.Sdk
{
  public class SetupDelegateAttributeFullName
  {
    static readonly string setupDelegateAttributeName = typeof(SetupDelegateAttribute).FullName;

    public static implicit operator string(SetupDelegateAttributeFullName fullName)
    {
      return setupDelegateAttributeName;
    }
  }
}