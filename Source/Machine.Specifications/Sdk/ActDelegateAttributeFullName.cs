namespace Machine.Specifications.Sdk
{
  public class ActDelegateAttributeFullName
  {
    static readonly string actDelegateAttributeName = typeof(ActDelegateAttribute).FullName;

    public static implicit operator string(ActDelegateAttributeFullName fullName)
    {
      return actDelegateAttributeName;
    }
  }
}