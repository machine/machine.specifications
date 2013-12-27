namespace Machine.Specifications.Sdk
{
  public class CleanupDelegateAttributeFullName : AttributeFullName
  {
    static readonly string cleanupDelegateAttributeName = typeof(CleanupDelegateAttribute).FullName;

    public static implicit operator string(CleanupDelegateAttributeFullName fullName)
    {
      return cleanupDelegateAttributeName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}