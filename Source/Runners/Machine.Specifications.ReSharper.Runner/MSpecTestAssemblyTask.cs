namespace Machine.Specifications.ReSharperRunner
{
    using System;
    using System.Xml;

    using JetBrains.ReSharper.TaskRunnerFramework;

    [Serializable]
    public class MSpecTestAssemblyTask : RemoteTask, IEquatable<MSpecTestAssemblyTask>
    {
        private readonly string assemblyLocation;

        public MSpecTestAssemblyTask(string assemblyLocation)
            : base(RecursiveMSpecTaskRunner.RunnerId)
        {
            this.assemblyLocation = assemblyLocation;
        }

        // This constructor is used to rehydrate a task from an xml element. This is
        // used by the remote test runner's IsolatedAssemblyTestRunner, which creates
        // an app domain to isolate the test assembly from the remote process framework.
        // That framework retrieves these tasks from devenv/resharper via remoting (hence
        // the SerializableAttribute) but uses this hand rolled xml serialisation to
        // get the tasks into the app domain that will actually run the tests
        public MSpecTestAssemblyTask(XmlElement element)
            : base(element)
        {
            this.assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
        }

        public string AssemblyLocation
        {
            get { return this.assemblyLocation; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "AssemblyLocation", this.assemblyLocation);
        }

        public bool Equals(MSpecTestAssemblyTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(this.assemblyLocation, other.assemblyLocation);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return this.Equals(remoteTask as MSpecTestAssemblyTask);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MSpecTestAssemblyTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                return this.assemblyLocation != null ? this.assemblyLocation.GetHashCode() : 0;
            }
        }

        public override bool IsMeaningfulTask
        {
            // This task doesn't correspond to an IUnitTestElement
            get { return false; }
        }
    }
}