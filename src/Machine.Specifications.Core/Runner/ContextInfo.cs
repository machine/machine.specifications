using System;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class ContextInfo : IEquatable<ContextInfo>
    {
        public ContextInfo(string name, string concern, string typeName, string typeNamespace, string assemblyName)
        {
            Concern = concern;
            Name = name;
            TypeName = typeName;
            AssemblyName = assemblyName;
            Namespace = typeNamespace;
        }

        public string Name { get; }

        public string Concern { get; }

        public string TypeName { get; }

        public string Namespace { get; }

        public string AssemblyName { get; }

        public string FullName
        {
            get
            {
                var line = string.Empty;

                if (!string.IsNullOrEmpty(Concern))
                {
                    line += Concern + ", ";
                }

                return line + Name;
            }
        }

        public string CapturedOutput { get; set; }

        public bool Equals(ContextInfo other)
        {
            return other != null &&
                   other.Name == Name &&
                   other.Concern == Concern &&
                   other.TypeName == TypeName &&
                   other.Namespace == Namespace &&
                   other.AssemblyName == AssemblyName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContextInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(Name)
                .And(Concern)
                .And(TypeName)
                .And(Namespace)
                .And(AssemblyName);
        }
    }
}
