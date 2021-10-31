using System;

namespace Machine.Specifications.Model
{
    public class Tag : IEquatable<Tag>
    {
        public Tag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Equals(Tag other)
        {
            return other != null && other.Name == Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Tag);
        }

        public override int GetHashCode()
        {
            return Name != null
                ? Name.GetHashCode()
                : 0;
        }
    }
}
