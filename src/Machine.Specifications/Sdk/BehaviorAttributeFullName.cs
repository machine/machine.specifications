namespace Machine.Specifications.Sdk
{
  public class BehaviorAttributeFullName : AttributeFullName
  {
    static readonly string behaviorAttributeFullName = typeof(BehaviorsAttribute).FullName;

    public static implicit operator string(BehaviorAttributeFullName fullName)
    {
      return behaviorAttributeFullName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}