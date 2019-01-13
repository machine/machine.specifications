using System;

namespace Machine.Specifications.Sdk
{
  public abstract class AttributeFullName : IEquatable<AttributeFullName>
  {
    public abstract string FullName { get; }

    public bool Equals(AttributeFullName other)
    {
      return other.FullName == FullName;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != GetType())
      {
        return false;
      }

      AttributeFullName other = (AttributeFullName)obj;
      return Equals(other);
    }

    public override int GetHashCode()
    {
      return FullName.GetHashCode();
    }

    public static bool operator ==(AttributeFullName left, AttributeFullName right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(AttributeFullName left, AttributeFullName right)
    {
      return !Equals(left, right);
    }
  }
}