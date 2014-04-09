using System;
using System.IO;

namespace Machine.Specifications.Runner.Utility
{
    public class SpecAssemblyPath
    {
        private readonly string specAssemblyPath;

        public SpecAssemblyPath(string specAssemblyPath)
        {
            this.specAssemblyPath = specAssemblyPath;
            if (!File.Exists(specAssemblyPath))
            {
                throw new ArgumentException("Provided spec assembly path is not a proper path.", specAssemblyPath);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecAssemblyPath) obj);
        }

        public override int GetHashCode()
        {
            return specAssemblyPath.GetHashCode();
        }

        protected bool Equals(SpecAssemblyPath other)
        {
            return string.Equals(specAssemblyPath, other.specAssemblyPath);
        }

        public static bool operator ==(SpecAssemblyPath left, SpecAssemblyPath right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpecAssemblyPath left, SpecAssemblyPath right)
        {
            return !Equals(left, right);
        }

        public static implicit operator string(SpecAssemblyPath path)
        {
            return path.specAssemblyPath;
        }
    }
}