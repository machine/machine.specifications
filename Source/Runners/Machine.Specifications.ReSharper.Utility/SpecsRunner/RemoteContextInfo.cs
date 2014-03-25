namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;

    [Serializable]
    public class RemoteContextInfo
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

        public RemoteContextInfo()
        {
        }

        public RemoteContextInfo(string name, string concern, string typeName, string @namespace, string assemblyName)
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

            if (obj.GetType() != typeof(RemoteContextInfo))
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