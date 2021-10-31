using System;

namespace Machine.Specifications.Sdk
{
    public abstract class AttributeFullName : IEquatable<AttributeFullName>
    {
        public static bool operator ==(AttributeFullName left, AttributeFullName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AttributeFullName left, AttributeFullName right)
        {
            return !Equals(left, right);
        }

        public abstract string FullName { get; }

        public bool Equals(AttributeFullName other)
        {
            return other != null && other.FullName == FullName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AttributeFullName);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}
