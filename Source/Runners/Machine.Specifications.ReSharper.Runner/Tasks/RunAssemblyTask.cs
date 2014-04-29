using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
    [Serializable]
    public class RunAssemblyTask : Task, IEquatable<RunAssemblyTask>
    {
        public RunAssemblyTask(XmlElement element)
            : base(element)
        {
        }

        public RunAssemblyTask(string providerId, string assemblyLocation)
            : base(providerId, assemblyLocation)
        {
        }

        public override bool IsMeaningfulTask
        {
            get { return false; }
        }

        public bool Equals(RunAssemblyTask other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(AssemblyLocation, other.AssemblyLocation);
        }

        public override string ContextTypeName { get; set; }
        public override string BehaviorTypeName { get; set; }
        public override string SpecificationFieldName { get; set; }

        public override bool Equals(object other)
        {
            return Equals(other as RunAssemblyTask);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}