namespace Machine.Specifications.Sdk
{
  public class SetupDelegateAttributeFullName : AttributeFullName
  {
    static readonly string setupDelegateAttributeName = typeof(SetupDelegateAttribute).FullName;

    public static implicit operator string(SetupDelegateAttributeFullName fullName)
    {
      return setupDelegateAttributeName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}