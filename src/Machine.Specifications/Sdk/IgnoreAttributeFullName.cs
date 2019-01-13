namespace Machine.Specifications.Sdk
{
  public class IgnoreAttributeFullName : AttributeFullName
  {
    static readonly string ignoreAttributeFullName = typeof(IgnoreAttribute).FullName;

    public static implicit operator string(IgnoreAttributeFullName fullName)
    {
      return ignoreAttributeFullName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}