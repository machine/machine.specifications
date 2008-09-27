namespace Machine.Specifications.Model
{
  public class Tag
  {
    readonly string _name;
    public string Name { get { return _name; } }

    public Tag(string name)
    {
      _name = name;
    }

    public bool Equals(Tag obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj._name, _name);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(Tag)) return false;
      return Equals((Tag) obj);
    }

    public override int GetHashCode()
    {
      return (_name != null ? _name.GetHashCode() : 0);
    }
  }
}
