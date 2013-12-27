namespace Machine.Specifications.Sdk
{
  public class BehaviorDelegateAttributeFullName : AttributeFullName
  {
    static readonly string behaviorDelegateAttribute = typeof(BehaviorDelegateAttribute).FullName;

    public static implicit operator string(BehaviorDelegateAttributeFullName fullName)
    {
      return behaviorDelegateAttribute;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}