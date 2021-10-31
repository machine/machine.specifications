using System;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class ContextInfo : IEquatable<ContextInfo>
    {
        public ContextInfo()
        {
        }

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

        public static ContextInfo Parse(string contextInfoXml)
        {
            var document = XDocument.Parse(contextInfoXml);
            var name = document.SafeGet<string>("/contextinfo/name");
            var concern = document.SafeGet<string>("/contextinfo/concern");
            var typeName = document.SafeGet<string>("/contextinfo/typename");
            var typeNamespace = document.SafeGet<string>("/contextinfo/namespace");
            var assemblyName = document.SafeGet<string>("/contextinfo/assemblyname");
            var capturedOutput = document.SafeGet<string>("/contextinfo/capturedoutput");

            return new ContextInfo(name, concern, typeName, typeNamespace, assemblyName)
            {
                CapturedOutput = capturedOutput,
            };
        }
    }
}
