namespace Machine.Specifications.Sdk
{
  public class ActDelegateAttributeFullName : AttributeFullName
  {
    static readonly string actDelegateAttributeName = typeof(ActDelegateAttribute).FullName;

    public static implicit operator string(ActDelegateAttributeFullName fullName)
    {
      return actDelegateAttributeName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}