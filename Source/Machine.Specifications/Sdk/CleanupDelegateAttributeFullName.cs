namespace Machine.Specifications.Sdk
{
  public class CleanupDelegateAttributeFullName
  {
    static readonly string cleanupDelegateAttributeName = typeof(CleanupDelegateAttribute).FullName;

    public static implicit operator string(CleanupDelegateAttributeFullName fullName)
    {
      return cleanupDelegateAttributeName;
    }
  }
}