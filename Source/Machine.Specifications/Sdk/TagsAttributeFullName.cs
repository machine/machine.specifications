namespace Machine.Specifications.Sdk
{
  public class TagsAttributeFullName
  {
    static readonly string tagsAttributeFullName = typeof(TagsAttribute).FullName;

    public static implicit operator string(TagsAttributeFullName fullName)
    {
      return tagsAttributeFullName;
    }
  }
}