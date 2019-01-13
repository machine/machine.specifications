namespace Machine.Specifications.Sdk
{
  public class TagsAttributeFullName : AttributeFullName
  {
    static readonly string tagsAttributeFullName = typeof(TagsAttribute).FullName;

    public static implicit operator string(TagsAttributeFullName fullName)
    {
      return tagsAttributeFullName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}