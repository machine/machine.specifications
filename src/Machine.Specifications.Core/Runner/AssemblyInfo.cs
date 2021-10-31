using System;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class AssemblyInfo : IEquatable<AssemblyInfo>
    {
        public AssemblyInfo(string name, string location)
        {
            Name = name;
            Location = location;
        }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public string CapturedOutput { get; set; }

        public bool Equals(AssemblyInfo other)
        {
            return other != null && other.Name == Name && other.Location == Location;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AssemblyInfo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Location != null ? Location.GetHashCode() : 0);
            }
        }
    }
}
