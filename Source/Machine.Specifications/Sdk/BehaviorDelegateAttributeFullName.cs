namespace Machine.Specifications.Sdk
{
  public class BehaviorDelegateAttributeFullName
  {
    static readonly string behaviorDelegateAttribute = typeof(BehaviorDelegateAttribute).FullName;

    public static implicit operator string(BehaviorDelegateAttributeFullName fullName)
    {
      return behaviorDelegateAttribute;
    }
     
  }
}