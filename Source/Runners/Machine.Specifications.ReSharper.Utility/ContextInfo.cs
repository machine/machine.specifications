using System;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class ContextInfo
    {
        public string Name { get; private set; }
        public string Concern { get; private set; }
        public string TypeName { get; private set; }
        public string Namespace { get; private set; }
        public string AssemblyName { get; private set; }

        public string FullName
        {
            get
            {
                string line = "";

                if (!String.IsNullOrEmpty(this.Concern))
                {
                    line += this.Concern + ", ";
                }

                return line + this.Name;
            }
        }

        public string CapturedOutput { get; set; }

        public ContextInfo()
        {
        }

        public ContextInfo(string name, string concern, string typeName, string @namespace, string assemblyName)
        {
            this.Concern = concern;
            this.Name = name;
            this.TypeName = typeName;
            this.AssemblyName = assemblyName;
            this.Namespace = @namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(ContextInfo))
            {
                return false;
            }

            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 29 + (this.Name != null ? this.Name.GetHashCode() : 0);
                hash = hash * 29 + (this.Concern != null ? this.Concern.GetHashCode() : 0);
                hash = hash * 29 + (this.TypeName != null ? this.TypeName.GetHashCode() : 0);
                hash = hash * 29 + (this.Namespace != null ? this.Namespace.GetHashCode() : 0);
                hash = hash * 29 + (this.AssemblyName != null ? this.AssemblyName.GetHashCode() : 0);

                return hash;
            }
        }

        public static ContextInfo Parse(string contextInfoXml)
        {
            var document = XDocument.Parse(contextInfoXml);
            var name = document.SafeGet<string>("/contextinfo/name");
            var concern = document.SafeGet<string>("/contextinfo/concern");
            var typeName = document.SafeGet<string>("/contextinfo/typename");
            var @namespace = document.SafeGet<string>("/contextinfo/namespace");
            var assemblyName = document.SafeGet<string>("/contextinfo/assemblyname");
            var capturedoutput = document.SafeGet<string>("/contextinfo/capturedoutput");

            return new ContextInfo(name, concern, typeName, @namespace, assemblyName)
                       {
                           CapturedOutput = capturedoutput,
                       };
        }
    }

    [Serializable]
    public class CapturedOutput
    {
        public string StdOut { get; private set; }
        public string StdError { get; private set; }
        public string DebugTrace { get; private set; }

        public CapturedOutput(string stdOut, string stdError, string debugTrace)
        {
            this.StdOut = stdOut;
            this.StdError = stdError;
            this.DebugTrace = debugTrace;
        }
    }
}