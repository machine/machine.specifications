namespace Machine.Specifications.Sdk
{
  public class SubjectAttributeFullName : AttributeFullName
  {
    static readonly string subjectAttributeFullName = typeof(SubjectAttribute).FullName;

    public static implicit operator string(SubjectAttributeFullName fullName)
    {
      return subjectAttributeFullName;
    }

    public override string FullName
    {
      get { return this; }
    }
  }
}