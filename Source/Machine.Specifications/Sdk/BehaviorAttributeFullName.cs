namespace Machine.Specifications.Sdk
{
  public class BehaviorAttributeFullName
  {
    static readonly string behaviorAttributeFullName = typeof(BehaviorsAttribute).FullName;

    public static implicit operator string(BehaviorAttributeFullName fullName)
    {
      return behaviorAttributeFullName;
    }
  }
}