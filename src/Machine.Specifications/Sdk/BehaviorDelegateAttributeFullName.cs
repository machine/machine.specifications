namespace Machine.Specifications.Sdk
{
  public class BehaviorDelegateAttributeFullName : DelegateAttributeFullName
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