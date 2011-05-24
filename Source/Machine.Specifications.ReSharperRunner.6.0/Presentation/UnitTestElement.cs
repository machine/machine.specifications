using System;
using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
    internal abstract class UnitTestElement: IUnitTestElement
    {
        public string TypeName { get; protected set; }
        public string AssemblyLocation { get; private set; }

        public UnitTestElement(IUnitTestProvider provider, string typeName, string shortName, string assemblyLocation)
        {
            Provider = provider;
            TypeName = typeName;
            ShortName = shortName;
            AssemblyLocation = assemblyLocation;

            Children = new List<IUnitTestElement>();
            State = UnitTestElementState.Valid;
        }

        public bool Equals(IUnitTestElement other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other.GetType() == GetType())
            {
                var element = (UnitTestElement)other;
                return other.ShortName == ShortName && other.Provider == Provider
                    && element.AssemblyLocation == AssemblyLocation;
            }
            return false;
        }

      public abstract IProject GetProject();
      public abstract string GetPresentation();
      public abstract UnitTestNamespace GetNamespace();
      public abstract UnitTestElementDisposition GetDisposition();
      public abstract IDeclaredElement GetDeclaredElement();

      public IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
        {
            // TODO: HADI
            var unitTestTasks = new List<UnitTestTask>();

            return unitTestTasks;
        }

      public abstract string Kind { get; }
      public abstract IEnumerable<UnitTestElementCategory> Categories { get; }
      public abstract string ExplicitReason { get; }

      public string Id
        {
            get { return TypeName; }
        }

        public IUnitTestProvider Provider { get; private set; }
        public IUnitTestElement Parent { get; set; }
        public ICollection<IUnitTestElement> Children { get; private set; }
        public string ShortName { get; private set; }
        
        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State { get; set; }
    }
}