using System;
using System.IO;

namespace Machine.Specifications.Runner.Utility
{
    public class AssemblyPath : IEquatable<AssemblyPath>
    {
        private readonly string specAssemblyPath;

        public AssemblyPath(string specAssemblyPath)
        {
            this.specAssemblyPath = specAssemblyPath;

            if (!File.Exists(specAssemblyPath))
            {
                throw new ArgumentException("Provided spec assembly path is not a proper path.", specAssemblyPath);
            }
        }

        public static bool operator ==(AssemblyPath left, AssemblyPath right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssemblyPath left, AssemblyPath right)
        {
            return !Equals(left, right);
        }

        public static implicit operator string(AssemblyPath path)
        {
            return path.specAssemblyPath;
        }

        public bool Equals(AssemblyPath other)
        {
            return other != null && other.specAssemblyPath == specAssemblyPath;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AssemblyPath);
        }

        public override int GetHashCode()
        {
            return specAssemblyPath.GetHashCode();
        }

        public override string ToString()
        {
            return specAssemblyPath;
        }
    }
}
